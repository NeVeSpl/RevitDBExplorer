using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class PrintManager_SubmitPrint : MemberAccessorByType<PrintManager>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(PrintManager.SubmitPrint);

        IMemberAccessor IHaveFactoryMethod.Create() => new PrintManager_SubmitPrint();


        protected override bool CanBeSnoooped(Document document, PrintManager value) => false;
        protected override string GetLabel(Document document, PrintManager value) => "I wouldn't do that if I were you";
    }
}