using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using SimMetrics.Net;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal enum LookupFor { Category = 1, Class = 2, ElementId = 4, Parameter = 8,  All = 255}
    internal record ParameterSpec(BuiltInParameter BuiltInParameter, StorageType StorageType);
        

    internal static class FuzzySearchEngine
    {
        static readonly List<(string, BuiltInCategory)> Categories;
        static readonly List<(string, Type)> Classes;
        static readonly List<(string, ForgeTypeId)> Parameters;
        static readonly HashSet<string> ClassesBlackList = new()
        {
            //typeof(Material),
            //typeof(CurveElement).FullName,
            //typeof(ConnectorElement).FullName,
            //typeof(HostedSweep).FullName, 
            typeof(Room).FullName,
            typeof(Space).FullName,
            typeof(Area).FullName,
            typeof(RoomTag).FullName,
            typeof(SpaceTag).FullName,
            typeof(AreaTag).FullName,
            typeof(CombinableElement).FullName,
            typeof(Mullion).FullName,
            typeof(Panel).FullName,
            typeof(AnnotationSymbol).FullName,
            //typeof(AreaReinforcementType).FullName,
            //typeof(PathReinforcementType).FullName,
            typeof(AnnotationSymbolType).FullName,
            typeof(RoomTagType).FullName,
            typeof(SpaceTagType).FullName,
            typeof(AreaTagType).FullName,
            typeof(TrussType).FullName,
            typeof(Element).FullName,
            typeof(ElementType).FullName,
        };


        static FuzzySearchEngine()
        {
            Categories = LoadCategories();
            Classes = LoadClasses();
            Parameters = LoadParameters();
        }      
        public static void Init()
        {

        }


        public static IEnumerable<ILookupResult> Lookup(Document document,string text, LookupFor lookupFor = LookupFor.All)
        {
            var needle = text.Clean();
            var found = new List<ILookupResult>();

            if (lookupFor.HasFlag(LookupFor.ElementId))
            {
                var words = text.Split(' ');
                foreach (var word in words)
                {
                    if (int.TryParse(word, out int intValue))
                    {
                        found.Add(new LookupResult<ElementId>(new ElementId(intValue), 1.0) { Name = $"new ElementId({intValue})" });
                    }
                }
            }

            if (lookupFor.HasFlag(LookupFor.Category))
            {
                foreach (var item in Categories)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.61)
                    {
                        found.Add(new LookupResult<BuiltInCategory>(item.Item2, score){ Name = $"BuiltInCategory.{item.Item2}" });
                    }
                }
            }

            if (lookupFor.HasFlag(LookupFor.Class))
            { 
                foreach (var item in Classes)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.67)
                    {
                        found.Add(new LookupResult<Type>(item.Item2, score) { Name = $"typeof({item.Item2.Name})" });
                    }
                }
            }

            if (lookupFor.HasFlag(LookupFor.Parameter))
            {
                foreach (var item in Parameters)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.69)
                    {
                        var builtInParam = ParameterUtils.GetBuiltInParameter(item.Item2);
                        var storageType = document.get_TypeOfStorage(builtInParam);
                        var param = new ParameterSpec(builtInParam, storageType);
                        found.Add(new LookupResult<ParameterSpec>(param, score) { Name = $"BuiltInParameter.{builtInParam}" });
                    }
                }
            }

            if (found.Any())
            {
                var sorted = found.OrderByDescending(x => x.LevensteinScore);

                double prevScore = sorted.First().LevensteinScore;
                foreach (var item in sorted.Take(7))
                {
                    if (Math.Abs(item.LevensteinScore - prevScore) < 0.11)
                    {
                        yield return item;
                    }
                }
            }
        }

        private static List<(string, BuiltInCategory)> LoadCategories()
        {
            var allFilterableCategories = ParameterFilterUtilities.GetAllFilterableCategories();
            var categories = new List<(string, BuiltInCategory)>(allFilterableCategories.Count * 2);

            foreach (ElementId categoryId in allFilterableCategories)
            {
                var category = (BuiltInCategory)categoryId.IntegerValue;
                var label = LabelUtils.GetLabelFor(category);

                categories.Add((label.Clean(), category));
                categories.Add((category.ToString().Clean(), category));
            }

            return categories;
        }
        private static List<(string, Type)> LoadClasses()
        {
            var elementType = typeof(Element);
            var classes = elementType.Assembly.GetExportedTypes().Where(p => elementType.IsAssignableFrom(p) && !p.IsInterface).Where(x => !ClassesBlackList.Contains(x.FullName)).Select(x => (x.Name.Clean(), x)).ToList();
            return classes;
        }
        private static List<(string, ForgeTypeId)> LoadParameters()
        {
            var ids = ParameterUtils.GetAllBuiltInParameters();
            var parameters = new List<(string, ForgeTypeId)>(ids.Count * 2);
            foreach (var id in ids)
            {
                var label = LabelUtils.GetLabelForBuiltInParameter(id);
                var param = ParameterUtils.GetBuiltInParameter(id);

                if (param == BuiltInParameter.INVALID)
                {
                    continue;
                }

                var labelCleaned = label.Clean();
                var paramCleaned = param.ToString().Clean();

                if (!string.IsNullOrEmpty(labelCleaned))
                {
                    parameters.Add((labelCleaned, id));
                }
                if (!string.IsNullOrEmpty(paramCleaned))
                {
                    parameters.Add((paramCleaned.ToString().Clean(), id)); 
                } 
            }
            return parameters;
        }

        private static string Clean(this string text)
        {
            return text.RemoveWhitespace().ToLower();
        }
    }


    internal interface ILookupResult
    {
        string Name { get;  }
        double LevensteinScore { get;  }
        bool IsCategory { get; }
        bool IsClass { get; }
        bool IsElementId { get; }
        bool IsParameter { get; }
    }


    internal class LookupResult<T> : ILookupResult
    {
        public T  Value { get; init; }
        public double LevensteinScore { get; init; }
        public string Name { get; init; }
        public bool IsCategory { get; init; }
        public bool IsClass { get; init; }
        public bool IsElementId { get; init; }
        public bool IsParameter { get; init; }


        public LookupResult(T value, double levensteinScore)
        {
            Value = value;
            LevensteinScore = levensteinScore;
            IsCategory = typeof(T) == typeof(BuiltInCategory);
            IsClass = typeof(T) == typeof(Type);
            IsElementId = typeof(T) == typeof(ElementId);
            IsParameter = typeof(T) == typeof(ParameterSpec);
        }
    }
}