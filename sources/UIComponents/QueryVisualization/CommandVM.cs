using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.ICommand;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.QueryVisualization
{
    internal class CommandVM : BaseViewModel, IEquatable<CommandVM>
    {
        private readonly RDQCommand command;
        private readonly string args;
        private string name;
        private string filterName;
        private bool toRemove;      


        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }   
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
        public string FilterName
        {
            get
            {
                return filterName;
            }
            set
            {
                filterName = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<ICommandArgument> Arguments => command.MatchedArguments;
        public CmdType Type { get; set; }


        public CommandVM(RDQCommand command)
        {
            this.command = command; 
            args = String.Join(", ", command.MatchedArguments.Select(x => x.Name));

            if (command is ICommandForVisualization cmdforVisualization) 
            {
                Name = cmdforVisualization.Label;
                FilterName = cmdforVisualization.Description;
                Type = cmdforVisualization.Type;
            }
        }

        public bool Equals(CommandVM other)
        {
            return string.Equals(Name, other.Name) && string.Equals(args.ToString(), other.args.ToString()); 
        }
    }
}