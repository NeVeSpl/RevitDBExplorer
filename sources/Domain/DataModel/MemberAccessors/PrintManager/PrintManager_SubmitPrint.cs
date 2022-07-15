using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class PrintManager_SubmitPrint : MemberAccessorByType<PrintManager>, IHaveFactoryMethod
    {       
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (PrintManager x) => x.SubmitPrint(); yield return (PrintManager x, View v) => x.SubmitPrint(v); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new PrintManager_SubmitPrint();


        protected override bool CanBeSnoooped(Document document, PrintManager value) => false;
        protected override string GetLabel(Document document, PrintManager value) => "'I wouldn't do that if I were you' - Anthony";
    }
}