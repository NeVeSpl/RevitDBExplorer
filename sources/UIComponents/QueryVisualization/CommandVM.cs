using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.ICommand;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.QueryVisualization
{
    internal class CommandVM : BaseViewModel, IEquatable<CommandVM>
    {
        private readonly RDQCommand command;
        private readonly string argsHash;   
        private bool toRemove;


        public string Label { get; init; }
        public string Description { get; init; }
        public string APIDescription { get; init; }
        public CmdType Type { get; init; }
        public IEnumerable<IFuzzySearchResult> Arguments => command.MatchedArguments;
        public bool ToRemove
        {
            get
            {
                return toRemove;
            }
            set
            {
                toRemove = value;
                OnPropertyChanged();
            }
        }
        
        

        public CommandVM(RDQCommand command)
        {
            this.command = command;
            this.argsHash = String.Join(", ", command.Arguments.Select(x => x.Name));

            if (command is ICommandForVisualization cmdforVisualization) 
            {
                Label = cmdforVisualization.Label;
                Description = cmdforVisualization.Description;
                APIDescription = cmdforVisualization.APIDescription;
                Type = cmdforVisualization.Type;
            }
        }

        public bool Equals(CommandVM other)
        {
            return string.Equals(Label, other.Label) && string.Equals(argsHash.ToString(), other.argsHash.ToString()); 
        }
    }
}