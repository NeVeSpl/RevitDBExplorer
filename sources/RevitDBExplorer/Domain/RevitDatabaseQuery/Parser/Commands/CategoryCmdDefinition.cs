using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class CategoryCmdDefinition : ICommandDefinition, INeedInitialization, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("c: ", "c:[category]", "select elements of given category", AutocompleteItemGroups.Commands);
        private readonly DataBucket<CategoryCmdArgument> dataBucket = new DataBucket<CategoryCmdArgument>(0.59);


        public void Init()
        {
            var allFilterableCategories = ParameterFilterUtilities.GetAllFilterableCategories();
           
            foreach (ElementId categoryId in allFilterableCategories)
            {
                var category = (BuiltInCategory)categoryId.Value();
                var label = LabelUtils.GetLabelFor(category);

                var strCategory = category.ToString();

                if (!Category.IsBuiltInCategoryValid(category))
                {
                    continue;
                }
                dataBucket.Add(new AutocompleteItem(strCategory, strCategory, label), new CategoryCmdArgument(category), label, strCategory);             
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
            yield return "c";
            yield return "cat";
            yield return "category";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            if (argument.StartsWith(nameof(BuiltInCategory), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        public bool CanParticipateInGenericSearch() => true;


        public ICommand Create(string cmdText, string argument)
        {
            var arg = argument.RemovePrefix("BuiltInCategory.");
            var args = dataBucket.FuzzySearch(arg);
            return new CategoryCmd(cmdText, args);
        }
    }


    internal class CategoryCmdArgument : CommandArgument<BuiltInCategory>
    {
        public CategoryCmdArgument(BuiltInCategory value) : base(value)
        {            
            Name = $"BuiltInCategory.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
    }


    internal class CategoryCmd : Command, ICommandForVisualization
    {
        public string Label => String.Join(", ", Arguments.Select(x => x.Name));
        public string Description => "A filter used to find elements whose category matches any of a given set of categories.";
        public string APIDescription => "collector.OfCategory() or new ElementMulticategoryFilter()";
        public CmdType Type => CmdType.Category;


        public CategoryCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments) : base(text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = true;
        }        
    }
}