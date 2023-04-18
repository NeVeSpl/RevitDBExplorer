using RevitDBExplorer.Domain.Presentation;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree.Items
{
    internal class TreeItemsCommands
    {
        public SelectInRevitCommand SelectInRevit { get; } = SelectInRevitCommand.Instance;
        public ShowInRevitCommand ShowInRevit { get; } = ShowInRevitCommand.Instance;
        public SnoopInNewWindowCommand SnoopInNewWindow { get; } = SnoopInNewWindowCommand.Instance;
        public IsolateInRevitCommand IsolateInRevit { get; } = IsolateInRevitCommand.Instance;
        public DrawInRevitCommand DrawInRevit { get; } = DrawInRevitCommand.Instance;
        public DrawInRevitWithAVFCommand DrawInRevitAVF { get; } = DrawInRevitWithAVFCommand.Instance;
        public RelayCommand UseAsInpputForRDSCommand { get; }
        public RelayCommand GenerateUpdateQueryRDSCommand { get; }


        public TreeItemsCommands(RelayCommand useAsInpputForRDSCommand, RelayCommand generateUpdateQueryRDSCommand)
        {
            UseAsInpputForRDSCommand = useAsInpputForRDSCommand;
            GenerateUpdateQueryRDSCommand = generateUpdateQueryRDSCommand;
        }
    }
}