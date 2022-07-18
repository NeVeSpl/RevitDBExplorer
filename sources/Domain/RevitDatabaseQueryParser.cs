using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using SimMetrics.Net;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class RevitDatabaseQueryParser
    {
        static readonly List<(string, object)> Categories;
        static readonly List<(string, object)> Classes;
        static readonly HashSet<string> ClassesBlackList = new HashSet<string>
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


        static RevitDatabaseQueryParser()
        {   
            var allFilterableCategories =  ParameterFilterUtilities.GetAllFilterableCategories();
            Categories = new List<(string, object)>(allFilterableCategories.Count);

            foreach (ElementId categoryId in allFilterableCategories)
            {
                var category = (BuiltInCategory)categoryId.IntegerValue;
                var label = LabelUtils.GetLabelFor(category);
                Categories.Add((label.ToLower().RemoveWhitespace(), category));
                Categories.Add((category.ToString().ToLower().RemoveWhitespace(), category));                
            }

            var elementType = typeof(Element);
            Classes = elementType.Assembly.GetExportedTypes().Where(p => elementType.IsAssignableFrom(p) && !p.IsInterface).Where(x => !ClassesBlackList.Contains(x.FullName)).Select(x => (x.Name.ToLower(), (object)x)).ToList();
        }

        public static void Init()
        {

        }


        public static Result Parse(Document document, string query)
        {
            var commands = ParseQuery(query);
            
            var c1 = CreateCollector(document, commands);
            var c2 = WhereElementIsElementTypeOrNot(c1, commands);
            var c3 = OfClass(c2, commands);
            var c4 = OfCategory(c3, commands);
            var c5 = OfName(c4, commands);

            return c5;
        }

        private static List<Command> ParseQuery(string query)
        {
            var splitted = query.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var commands = splitted.Select(f => new Command(f)).ToList();

            return commands;
        }
        private static Result CreateCollector(Document document, List<Command> commands)
        {
            if (commands.Any(x => x.Type == CmdType.ActiveView))
            {
                var c = new FilteredElementCollector(document, document.ActiveView.Id);
                var s = "new FilteredElementCollector(document, document.ActiveView.Id)";
                return new Result(c, s);
            }

            if (commands.Any(x => x.Type == CmdType.Id))
            {
                var ids = commands.Where(x => x.Type == CmdType.Id).Select(x => x.Argument);
                List<ElementId> elementids = new List<ElementId>();
                foreach (var id in ids)
                {
                    if (int.TryParse(id, out int intId))
                    {
                        elementids.Add(new ElementId(intId));
                    }
                }
                if (elementids.Any())
                {
                    var c = new FilteredElementCollector(document, elementids);
                    var s = "new FilteredElementCollector(document, new [] {" + String.Join(", ", elementids.Select(x => $"new ElementId({x.IntegerValue})")) + "})";
                    return new Result(c, s);
                }
            }

            {
                var c = new FilteredElementCollector(document);
                var s = "new FilteredElementCollector(document)";
                return new Result(c, s);
            }
        }
        private static Result WhereElementIsElementTypeOrNot(Result token, List<Command> commands)
        {
            HashSet<CmdType> quickFilters = new HashSet<CmdType>() { CmdType.ElementType, CmdType.NotElementType, CmdType.Category, CmdType.Class };
            if (commands.Any(x => quickFilters.Contains(x.Type)))
            {
                var isElementTypePresent = commands.Any(x => x.Type == CmdType.ElementType);
                var isNotElementTypePresent = commands.Any(x => x.Type == CmdType.NotElementType);

                if (isElementTypePresent == true && isNotElementTypePresent == false)
                {
                    var c = token.Collector.WhereElementIsElementType();
                    var s = token.RevitAPIQuery + ".WhereElementIsElementType()";
                    return new Result(c, s);
                }
                if (isElementTypePresent == false && isNotElementTypePresent == true)
                {
                    var c = token.Collector.WhereElementIsNotElementType();
                    var s = token.RevitAPIQuery + ".WhereElementIsNotElementType()";
                    return new Result(c, s);
                }
                if (isElementTypePresent == false && isNotElementTypePresent == false)
                {
                    return token;
                }
            }

            {
                var c = token.Collector.WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)));
                var s = token.RevitAPIQuery + ".WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)))";

                return new Result(c, s);
            }
        }
        private static Result OfClass(Result token, List<Command> commands)
        {
            if (commands.Any(x => x.Type == CmdType.Class))
            {
                var cmd = commands.Where(x => x.Type == CmdType.Class).First();
                var c = token.Collector.OfClass(cmd.ArgumentAsType);
                var s = token.RevitAPIQuery + $".OfClass(typeof({cmd.ArgumentAsType.Name}))";
                return new Result(c, s);
            }
            return token;
        }
        private static Result OfCategory(Result token, List<Command> commands)
        {
            if (commands.Any(x => x.Type == CmdType.Category))
            {
                var cmd = commands.Where(x => x.Type == CmdType.Category).First();
                var c = token.Collector.OfCategory(cmd.ArgumentAsCategory);
                var s = token.RevitAPIQuery + $".OfCategory(BuiltInCategory.{cmd.ArgumentAsCategory})";
                return new Result(c, s);
            }
            
            return token;
        }
        private static Result OfName(Result token, List<Command> commands)
        {
            if (commands.Any(x => x.Type == CmdType.WhoKnows))
            {
                var cmd = commands.Where(x => x.Type == CmdType.WhoKnows).First(); 
                var rules = new[]
                {
                    ParameterFilterRuleFactory.CreateContainsRule(new ElementId(BuiltInParameter.ALL_MODEL_TYPE_NAME), cmd.Argument),
                    ParameterFilterRuleFactory.CreateContainsRule(new ElementId(BuiltInParameter.ALL_MODEL_MARK), cmd.Argument),
                    ParameterFilterRuleFactory.CreateContainsRule(new ElementId(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM), cmd.Argument),
                };
                
                var or = new LogicalOrFilter(rules.Select(x => new ElementParameterFilter(x, false)).ToList<ElementFilter>());               
                var c = token.Collector.WherePasses(or);
                var s = token.RevitAPIQuery + $".WherePasses(Name=%{cmd.Argument}%)";
                return new Result(c, s);
            }
            return token;
        }


        private enum CmdType
        {
            ActiveView,
            View,
            Id,
            ElementType,
            NotElementType,
            Category,
            Class,
            NameParam,
            WhoKnows = 666
        }

        private class Command
        {
            public CmdType Type = CmdType.WhoKnows;
            public string Argument;
            public Type ArgumentAsType;
            public int ArgumentAsInt;
            public BuiltInCategory ArgumentAsCategory;


            public Command(string cmd)
            {
                var splitted = cmd.Split(new[] { ':' }, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Length == 1)
                {
                    Type = InterpretCmdType(splitted[0]);
                    if (Type == CmdType.WhoKnows)
                    {
                        Type = InterfereCmdType(splitted[0]);
                    }
                    Argument = splitted[0].Trim();
                }
                else
                {
                    // todo
                    //Type = InterpretCmdType(splitted[0]);
                    Argument = splitted[1].Trim();
                }

            }


            private CmdType InterfereCmdType(string strType)
            {
                if (int.TryParse(strType, out ArgumentAsInt))
                {                    
                    return CmdType.Id;
                }

                double maxScore = 0;
                object result = null;
                var needle = strType.ToLower().RemoveWhitespace();
                foreach (var item in Classes.Concat(Categories))
                {
                    var score = needle.ApproximatelyEquals(item.Item1, SimMetricType.Levenstein);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        result = item.Item2;
                    }
                }
                if (maxScore >= 0.69)
                {
                    if (result is Type)
                    {
                        ArgumentAsType = (Type)result;
                        return CmdType.Class;
                    }
                    if (result is BuiltInCategory)
                    {
                        ArgumentAsCategory = (BuiltInCategory)result;
                        return CmdType.Category;
                    }
                }

                return CmdType.WhoKnows;
            }
            private CmdType InterpretCmdType(string strType)
            {
                var needle = strType.ToLower().RemoveWhitespace();
                switch (needle)
                {
                    case "active":
                    //case "view":
                    case "activeview":
                        return CmdType.ActiveView;
                    //case "id":
                    //case "ids":
                    //    return CmdType.Id;
                    case "elementtype":
                    case "notelement":
                    case "type":
                    case "types":
                        return CmdType.ElementType;
                    case "element":
                    case "notelementtype":
                    case "elements":
                        return CmdType.NotElementType;
                    //case "category":
                    //case "cat":
                    //    return CmdType.Category;
                    //case "class":
                    //    return CmdType.Class;
                }

                return CmdType.WhoKnows;
            }
        }


        public record Result(FilteredElementCollector Collector, string RevitAPIQuery);
    }
}