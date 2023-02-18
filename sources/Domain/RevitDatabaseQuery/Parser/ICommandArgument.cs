// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal interface ICommandArgument
    {
        string Name { get;  }
        string Label { get; }       
       
    }

    internal abstract class CommandArgument<T> : ICommandArgument
    {
        public T Value { get; init; }
        public string Name { get; init; }
        public string Label { get; init; }     
        


        public CommandArgument(T value)
        {
            Value = value;           
        }
    }
}