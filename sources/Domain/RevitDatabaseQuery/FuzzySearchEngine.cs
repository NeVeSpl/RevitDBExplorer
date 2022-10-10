using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
using SimMetrics.Net;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal static class FuzzySearchEngine
    {
        internal enum LookFor { Category = 1, Class = 2, ElementId = 4, Parameter = 8, StructuralType = 16, Level = 32, RuleBasedFilter = 64, Room = 128, All = 255 }

        static readonly List<(string, BuiltInCategory)> Categories;
        static readonly List<(string, Type)> Classes;
        static readonly List<(string, BuiltInParameter)> Parameters;
        static List<(string, ElementId)> UserParameters = Enumerable.Empty<(string, ElementId)>().ToList();
        static readonly List<(string, StructuralType)> StructuralTypes;
        static readonly List<Bucket> Buckets = new List<Bucket>();


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
            StructuralTypes = LoadStructuralTypes();
            
        }      
        public static void Init()
        {

        }
        public static void LoadDocumentSpecificData(Document document)
        {            
            LoadUserParameters(document);
            Buckets.Clear();
            Buckets.Add(new Bucket(LookFor.Level, Load<Level>(document), (kv, score) => new LevelMatch(kv.Value, score, kv.Key)));
            Buckets.Add(new Bucket(LookFor.RuleBasedFilter, Load<ParameterFilterElement>(document), (kv, score) => new RuleMatch(kv.Value, score, kv.Key)));
            Buckets.Add(new Bucket(LookFor.Room, LoadRooms(document), (kv, score) => new RoomMatch(kv.Value, score, kv.Key)));
        }


        public static IEnumerable<ILookupResult> Lookup(string text, LookFor lookupFor = LookFor.All)
        {
            var needle = text.Clean();
            var found = new List<ILookupResult>();

            if (lookupFor.HasFlag(LookFor.ElementId))
            {
                //var words = text.Split(' ');
                //foreach (var word in words)
                {
                    if (int.TryParse(text, out int intValue))
                    {
                        found.Add(new ElementIdMatch(new ElementId(intValue), 1.0));
                    }
                }
            }

            if (lookupFor.HasFlag(LookFor.Category))
            {
                foreach (var item in Categories)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.59)
                    {
                        found.Add(new CategoryMatch(item.Item2, score));
                    }
                }
            }

            if (lookupFor.HasFlag(LookFor.Class))
            { 
                foreach (var item in Classes)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.61)
                    {
                        found.Add(new ClassMatch(item.Item2, score));
                    }
                }
            }

            if (lookupFor.HasFlag(LookFor.Parameter))
            {
                foreach (var item in Parameters)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.69)
                    {                       
                        found.Add(new ParameterMatch(item.Item2, score));
                    }
                }
                foreach (var item in UserParameters)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.67)
                    {                        
                        found.Add(new ParameterMatch(item.Item2, score, item.Item1));
                    }
                }
            }

            if (lookupFor.HasFlag(LookFor.StructuralType))
            {
                foreach (var item in StructuralTypes)
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > 0.61)
                    {
                        found.Add(new StructuralTypeMatch(item.Item2, score - 0.1));
                    }
                }
            }

            foreach (var bucket in Buckets)
            {
                if (lookupFor.HasFlag(bucket.Type))
                {
                    foreach (var item in bucket.Elements)
                    {
                        var score = needle.ApproximatelyEquals(item.Key, SimMetricType.Levenstein);
                        if (score > 0.61)
                        {
                            found.Add(bucket.CreateMatch(item, score));
                        }
                    }
                }
            }

            if (found.Any())
            {
                var sorted = found.OrderByDescending(x => x.LevensteinScore);

                double prevScore = sorted.First().LevensteinScore;
                foreach (var item in sorted.Take(27))
                {
                    if (Math.Abs(item.LevensteinScore - prevScore) < 0.13)
                    {
                        yield return item;
                    }
                    else
                    {
                        yield break;
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

                if (!Category.IsBuiltInCategoryValid(category))
                {
                    continue;
                }

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
        private static List<(string, BuiltInParameter)> LoadParameters()
        {
#if R2022b
            var ids = ParameterUtils.GetAllBuiltInParameters().Select(x => ParameterUtils.GetBuiltInParameter(x)).ToList();
#endif
#if R2021e
            var bips = System.Enum.GetValues(typeof(BuiltInParameter));
            var ids = new List<BuiltInParameter>(bips.Length);
            foreach (BuiltInParameter i in bips)
            {
                try
                {
                    var label = LabelUtils.GetLabelFor(i);
                    ids.Add(i);
                }
                catch
                {

                }
            }
#endif            
            var parameters = new List<(string, BuiltInParameter)>(ids.Count * 2);
            foreach (var param in ids)
            {
                var label = LabelUtils.GetLabelFor(param);               
                
                if (param == BuiltInParameter.INVALID)
                {
                    continue;
                }

                var labelCleaned = label.Clean();
                var paramCleaned = param.ToString().Clean();

                if (!string.IsNullOrEmpty(labelCleaned))
                {
                    parameters.Add((labelCleaned, param));
                }
                if (!string.IsNullOrEmpty(paramCleaned))
                {
                    parameters.Add((paramCleaned.ToString().Clean(), param)); 
                } 
            }
            return parameters;
        }
        private static void LoadUserParameters(Document document)
        {
            var newList = new List<(string, ElementId)>(UserParameters.Count);
            foreach (var userParam in new FilteredElementCollector(document).OfClass(typeof(ParameterElement)))
            {
                newList.Add((userParam.Name.Clean(), userParam.Id));
            }
            UserParameters = newList;
        }
        private static List<(string, StructuralType)> LoadStructuralTypes()
        {
            return new List<(string, StructuralType)>()
            {
                ("Beam".Clean(), StructuralType.Beam),
                ("Brace".Clean(), StructuralType.Brace),
                ("Column".Clean(), StructuralType.Column),
                ("Footing".Clean(), StructuralType.Footing),
                ("NonStructural".Clean(), StructuralType.NonStructural),
                ("UnknownFraming".Clean(), StructuralType.UnknownFraming),               
            };
        }
        private static List<KeyValue> Load<T>(Document document) where T : Element
        {
            var newList = new List<KeyValue>(UserParameters.Count);
            foreach (var level in new FilteredElementCollector(document).OfClass(typeof(T)))
            {
                newList.Add(new (level.Name.Clean(), level.Id));
            }
            return newList;
        }
        private static List<KeyValue> LoadRooms(Document document)
        {
            var newList = new List<KeyValue>(UserParameters.Count);
            foreach (var room in new FilteredElementCollector(document).WherePasses(new Autodesk.Revit.DB.Architecture.RoomFilter()))
            {
                newList.Add(new(room.Name.Clean(), room.Id));
            }
            return newList;
        }

        private static string Clean(this string text)
        {
            return text.RemoveWhitespace().ToLower();
        }

        private class Bucket
        {
            public LookFor Type { get; init; }
            public List<KeyValue> Elements { get; init; }
            public Func<KeyValue, double, ILookupResult> CreateMatch { get; init; }


            public Bucket(LookFor type, List<KeyValue> elements, Func<KeyValue, double, ILookupResult> createMatch)
            {
                Type = type;
                Elements = elements;
                CreateMatch = createMatch;
            }
        }

        private record struct KeyValue(string Key, ElementId Value);       
    }   
}