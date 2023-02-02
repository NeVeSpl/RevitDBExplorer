using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ClassCmdFactory : CommandFactory<ClassMatch>
    {
        public override IEnumerable<string> GetClassifiers()
        {
            yield return "t";
            yield return "type";
            yield return "class";
            yield return "typeof";
        }
        public override IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public override bool CanRecognizeArgument(string argument)
        {            
            return false;
        }


        public override ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            return new Command(CmdType.Class, cmdText, arguments, null) { IsBasedOnQuickFilter = true };
        }
        public override IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            return FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Class);
        }
    }


    internal class ClassMatch : LookupResult<Type>
    {
        public ClassMatch(Type value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.Class;
            Name = $"typeof({value.Name})";
            Label = value.Name;
        }
    }
}
