using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.ViewModels
{
    internal class CommandVM : BaseViewModel, IEquatable<CommandVM>
    {
        private readonly RDQCommand command;
        private Brush fill;
        private Brush foreground = Brushes.Black;
        private string name;
        private string filterName;
        private bool toRemove;
        private IEnumerable<ILookupResult> arguments;

        public Brush Fill
        {
            get
            {
                return fill;
            }
            set
            {
                fill = value;
                OnPropertyChanged();
            }
        }
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
        public Brush Foreground
        {
            get
            {
                return foreground;
            }
            set
            {
                foreground = value;
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
        public IEnumerable<ILookupResult> Arguments
        {
            get
            {
                return arguments;
            }
            set
            {
                arguments = value;
                OnPropertyChanged();
            }
        }


        public CommandVM(RDQCommand command)
        {
            this.command = command;          
            string hexColor = "#FFFFFF";

            string args = String.Join(", ", command.MatchedArguments.Select(x => x.Name));
            Arguments = command.MatchedArguments;
            switch (command.Type)
            {
                case CmdType.ActiveView:
                    Name = "active view";
                    FilterName = "new FilteredElementCollector(document, document.ActiveView.Id)";                    
                    hexColor = "#FFF2CC";
                    break;               
                case CmdType.ElementId:
                    Name = args;
                    FilterName = ".WherePasses(new ElementIdSetFilter())";
                    hexColor = "#FBE5D5";
                    break;
                case CmdType.ElementType:
                    Name = "element type";
                    FilterName = ".WhereElementIsElementType()";
                    hexColor = "#FFF2CC";
                    break;
                case CmdType.NotElementType:
                    Name = "element";
                    FilterName = ".WhereElementIsNotElementType()";
                    hexColor = "#FFF2CC";
                    break;
                case CmdType.Category:
                    Name = args;
                    FilterName = ".OfCategory()";
                    hexColor = "#DEEBF6";
                    break;
                case CmdType.Class:
                    Name = args;
                    FilterName = ".OfClass()";
                    hexColor = "#E2EFD9";
                    break;                
                case CmdType.Parameter:
                    var firstArg = command.MatchedArguments.OfType<BuiltInParameterMatch>().First();

                    string argsForParam = String.Join(", ", command.MatchedArguments.Take(1).Select(x => x.Name));
                    string count = "";
                    if (command.MatchedArguments.Count() > 1)
                    {
                        count = $" [+{command.MatchedArguments.Count() - 1} more]";
                    }
                    
                    Name = $"{argsForParam}{count} {command.Operator.ToString(firstArg.StorageType)}";
                    FilterName = ".WherePasses(new ElementParameterFilter())";
                    hexColor = "#EDEDED";
                    break;
                case CmdType.Incorrect:
                    Name = command.Text;
                    FilterName = "could not recognize phrase";
                    hexColor = "#FF0000";
                    Foreground = Brushes.White;
                    break;
                case CmdType.WhoKnows:
                    Name = args;
                    hexColor = "#E3D6EC";
                    break;
                default:
                    throw new NotImplementedException();
            };



            fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));


        }

        public bool Equals(CommandVM other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Fill.ToString(), other.Fill.ToString()); 
        }
    }
}