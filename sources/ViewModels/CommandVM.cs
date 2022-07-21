using System;
using System.Linq;
using System.Windows.Media;
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
        private bool toRemove;

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


        public CommandVM(RDQCommand command)
        {
            this.command = command;          
            string hexColor = "#FFFFFF";

            string args = String.Join(", ", command.Arguments.Select(x => x.Name));

            switch (command.Type)
            {
                case Domain.RevitDatabaseQuery.CmdType.ActiveView:
                    Name = "active view";
                    hexColor = "#FFF2CC";
                    break;               
                case Domain.RevitDatabaseQuery.CmdType.ElementId:
                    Name = args;
                    hexColor = "#FBE5D5";
                    break;
                case Domain.RevitDatabaseQuery.CmdType.ElementType:
                    Name = "element type";
                    hexColor = "#FFF2CC";
                    break;
                case Domain.RevitDatabaseQuery.CmdType.NotElementType:
                    Name = "element";
                    hexColor = "#FFF2CC";
                    break;
                case Domain.RevitDatabaseQuery.CmdType.Category:
                    Name = args;
                    hexColor = "#DEEBF6";
                    break;
                case Domain.RevitDatabaseQuery.CmdType.Class:
                    Name = args;
                    hexColor = "#E2EFD9";
                    break;
                case Domain.RevitDatabaseQuery.CmdType.NameParam:
                    Name = $"name: {command.Argument}";
                    hexColor = "#EDEDED";
                    break;
                case Domain.RevitDatabaseQuery.CmdType.Parameter:
                    Name = $"{args} {command.Operator} {command.OperatorArgumentAsString}";
                    hexColor = "#EDEDED";
                    break;
                case Domain.RevitDatabaseQuery.CmdType.Incorrect:
                    Name = command.Argument;
                    hexColor = "#FF0000";
                    Foreground = Brushes.White;
                    break;
                case Domain.RevitDatabaseQuery.CmdType.WhoKnows:
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