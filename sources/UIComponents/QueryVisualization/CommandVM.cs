using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
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
            var labels = String.Join(", ", command.MatchedArguments.Select(x => x.Label));
            switch (command.Type)
            {
                case CmdType.ActiveView:
                    Name = "active view";
                    FilterName = "new VisibleInViewFilter(document, document.ActiveView.Id)"; 
                    break;               
                case CmdType.ElementId:
                    Name = args;
                    FilterName = "new ElementIdSetFilter()";                   
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
                    var arguments = command.MatchedArguments.OfType<ParameterMatch>();
                    var firstArg = arguments.First();
                    
                    string count = "";
                    if (arguments.Count() > 1)
                    {
                        count = $" [+{arguments.Count() - 1} more]";
                    }
                    string name = firstArg.Name;
                    if(!firstArg.IsBuiltInParameter)
                    {
                        name = firstArg.Label;
                    }
                    
                    Name = $"{name}{count} {command.Operator.ToString(firstArg.StorageType)}";
                    FilterName = "new ElementParameterFilter()";                   
                    break;
                case CmdType.Incorrect:
                    Name = command.Text;
                    FilterName = "could not recognize phrase";  
                    break;
                case CmdType.Level:
                    Name = labels;
                    FilterName = "new ElementLevelFilter()";
                    break;
                case CmdType.StructuralType:
                    Name = args;
                    FilterName = "new ElementStructuralTypeFilter()";
                    break;
                case CmdType.Room:                   
                    Name = args;
                    FilterName = "new ElementIntersectsSolidFilter()";
                    break;
                case CmdType.RuleBasedFilter:                    
                    Name = "Rule-based filter: " + labels;
                    FilterName = "ParameterFilterElement.GetElementFilter()";
                    break;
                default:
                    Name = args;
                    break;
            };
        }

        public bool Equals(CommandVM other)
        {
            return string.Equals(Name, other.Name) && string.Equals(args.ToString(), other.args.ToString()); 
        }
    }
}