using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class LevelCmdDefinition : ICommandDefinition, INeedInitializationWithDocument
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("l: ", "l:[level]", "select elements from a given level");
        private readonly DataBucket<LevelCmdArgument> dataBucket = new DataBucket<LevelCmdArgument>(0.61);

      
        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var element in new FilteredElementCollector(document).OfClass(typeof(Level)))
            {
                dataBucket.Add(null, new LevelCmdArgument(element.Id, element.Name), element.Name);

            }
            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
      

        public IEnumerable<string> GetClassifiers()
        {
            yield return "l";
            yield return "lvl";
            yield return "level";         
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
            return new LevelCmd(cmdText, args);
        }
    }


    internal class LevelCmdArgument : CommandArgument<ElementId>
    {
        public LevelCmdArgument(ElementId levelId, string name) : base(levelId)
        {
            CmdType = CmdType.Level;
            Name = $"new ElementId({levelId})";
            Label = name;
        }
    }


    internal class LevelCmd : Command
    {
        public LevelCmd(string text, IEnumerable<ICommandArgument> matchedArguments = null) : base(CmdType.Level, text, matchedArguments, null)
        {
        }
    }
}