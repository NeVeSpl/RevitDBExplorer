using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Document_Close : MemberAccessorByType<Document>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(Document.Close);
        IMemberAccessor IHaveFactoryMethod.Create() => new Document_Close();

      
        protected override bool CanBeSnoooped(Document document, Document value) => false;
        protected override string GetLabel(Document document, Document value) => "cannot be done";       
    }
}