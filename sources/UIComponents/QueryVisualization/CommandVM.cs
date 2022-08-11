using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Command;

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
        public IEnumerable<ILookupResult> Arguments => command.MatchedArguments;
        public CmdType Type => command.Type;


        public CommandVM(RDQCommand command)
        {
            this.command = command; 
            args = String.Join(", ", command.MatchedArguments.Select(x => x.Name));          
            switch (command.Type)
            {
                case CmdType.ActiveView:
                    Name = "active view";
                    FilterName = "new FilteredElementCollector(document, document.ActiveView.Id)"; 
                    break;               
                case CmdType.ElementId:
                    Name = args;
                    FilterName = ".WherePasses(new ElementIdSetFilter())";                   
                    break;
                case CmdType.ElementType:
                    Name = "element type";
                    FilterName = ".WhereElementIsElementType()";                  
                    break;
                case CmdType.NotElementType:
                    Name = "element";
                    FilterName = ".WhereElementIsNotElementType()";                 
                    break;
                case CmdType.Category:
                    Name = args;
                    FilterName = ".OfCategory()";                 
                    break;
                case CmdType.Class:
                    Name = args;
                    FilterName = ".OfClass()";                   
                    break;                
                case CmdType.Parameter:
                    var firstArg = command.MatchedArguments.OfType<ParameterMatch>().First();

                    string argsForParam = String.Join(", ", command.MatchedArguments.Take(1).Select(x => x.Name));
                    string count = "";
                    if (command.MatchedArguments.Count() > 1)
                    {
                        count = $" [+{command.MatchedArguments.Count() - 1} more]";
                    }
                    
                    Name = $"{argsForParam}{count} {command.Operator.ToString(firstArg.StorageType)}";
                    FilterName = ".WherePasses(new ElementParameterFilter())";                   
                    break;
                case CmdType.Incorrect:
                    Name = command.Text;
                    FilterName = "could not recognize phrase";  
                    break;              
                
            };
        }

        public bool Equals(CommandVM other)
        {
            return string.Equals(Name, other.Name) && string.Equals(args.ToString(), other.args.ToString()); 
        }
    }
}