using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ClassCmdDefinition : ICommandDefinition, INeedInitialization, IOfferArgumentAutocompletion
    {
        private static readonly HashSet<string> ClassesBlackList = new()
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
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("t: ", "t:[class]", "select elements of given class", AutocompleteItemGroups.Commands);
        private readonly DataBucket<ClassCmdArgument> dataBucket = new DataBucket<ClassCmdArgument>(0.61);


        public void Init()
        {
            var elementType = typeof(Element);
            var classes = elementType.Assembly.GetExportedTypes()
                                              .Where(p => elementType.IsAssignableFrom(p) && !p.IsInterface)
                                              .Where(x => !ClassesBlackList.Contains(x.FullName));
            foreach (var @class in classes)
            {
                dataBucket.Add(new AutocompleteItem(@class.Name, @class.Name, ""), new ClassCmdArgument(@class), @class.Name);
            }
            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix)
        {
          
            return dataBucket.ProvideAutoCompletion(prefix);
        
        }


        public IEnumerable<string> GetClassifiers()
        {
            yield return "t";
            yield return "type";
            yield return "class";
            yield return "typeof";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => true;


        public ICommand Create(string cmdText, string argument)
        {           
            var args = dataBucket.FuzzySearch(argument);
            return new ClassCmd(cmdText, args);
        }
    }


    internal class ClassCmdArgument : CommandArgument<Type>
    {
        public ClassCmdArgument(Type value) : base(value)
        {           
            Name = $"typeof({value.Name})";
            Label = value.Name;
        }
    }


    internal class ClassCmd : Command, ICommandForVisualization
    {
        public string Label => String.Join(", ", Arguments.Select(x => x.Name));
        public string Description => "A filter used to match elements by their class.";
        public string APIDescription => "collector.OfClass() or new ElementMulticlassFilter()";
        public CmdType Type => CmdType.Class;


        public ClassCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}