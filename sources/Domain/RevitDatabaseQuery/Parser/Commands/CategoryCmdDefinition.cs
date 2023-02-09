using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class CategoryCmdDefinition : ICommandDefinition, INeedInitialization
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("c: ", "c:[category]", "select elements of given category");
        private readonly DataBucket<CategoryCmdArgument> dataBucket = new DataBucket<CategoryCmdArgument>(0.59);


        public void Init()
        {
            var allFilterableCategories = ParameterFilterUtilities.GetAllFilterableCategories();
           
            foreach (ElementId categoryId in allFilterableCategories)
            {
                var category = (BuiltInCategory)categoryId.Value();
                var label = LabelUtils.GetLabelFor(category);

                if (!Category.IsBuiltInCategoryValid(category))
                {
                    continue;
                }
                dataBucket.Add(null, new CategoryCmdArgument(category), label, category.ToString());             
            }

            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;


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
            CmdType = CmdType.Category;
            Name = $"BuiltInCategory.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
    }


    internal class CategoryCmd : Command
    {
        public CategoryCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments) : base(CmdType.Category, text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}