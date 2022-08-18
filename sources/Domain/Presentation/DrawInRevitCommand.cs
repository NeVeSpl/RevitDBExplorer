using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Tree;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Presentation
{
    internal class DrawInRevitCommand : BaseCommand
    {
        public static readonly DrawInRevitCommand Instance = new DrawInRevitCommand();

        public override bool CanExecute(object parameter)
        {
            return false;           
        }

        public override void Execute(object parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}
