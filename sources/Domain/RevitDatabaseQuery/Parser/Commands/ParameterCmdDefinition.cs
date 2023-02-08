using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ParameterCmdDefinition : ICommandDefinition, INeedInitialization, INeedInitializationWithDocument
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("p: ", "p:[parametr] = [value]", "search for a parameter (value)");
        private readonly DataBucket<ParameterMatch> dataBucket = new DataBucket<ParameterMatch>(0.69);
        private readonly DataBucket<ParameterMatch> dataBucketForUser = new DataBucket<ParameterMatch>(0.67);

        public void Init()
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
                dataBucket.Add(null, new ParameterMatch(param), label, param.ToString());              
            }
            dataBucket.Rebuild();
        }
        public void Init(Document document)
        {
            dataBucketForUser.Clear();         
            foreach (var userParam in new FilteredElementCollector(document).OfClass(typeof(ParameterElement)))
            {
                dataBucketForUser.Add(null, new ParameterMatch(userParam.Id, userParam.Name), userParam.Name);
            }
            dataBucketForUser.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
       

        public IEnumerable<string> GetClassifiers()
        {
            yield return "p";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            if (argument.StartsWith(nameof(BuiltInParameter), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (Operators.DoesContainAnyValidOperator(argument))
            {
                return true;
            }
            return false;
        }
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            var @operator = Operators.Parse(argument);
            var leftSide = Operators.GetLeftSideOfOperator(argument);
            var bareArgument = leftSide.RemovePrefix(nameof(BuiltInParameter));
            var argsBIP = dataBucket.FuzzySearch(bareArgument);
            var argsUser = dataBucketForUser.FuzzySearch(bareArgument);
            var args = argsBIP.Union(argsUser);

            return new ParameterCmd(cmdText, args, @operator);
        }       
    }


    internal class ParameterCmd : Command
    {
        public ParameterCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null, OperatorWithArgument @operator = null) : base(CmdType.Parameter, text, matchedArguments, @operator)
        {
        }
    }
}