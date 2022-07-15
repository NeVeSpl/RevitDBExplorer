using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ForgeTypeId_Clear : MemberAccessorByType<ForgeTypeId>, IHaveFactoryMethod
    {
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (ForgeTypeId x) => x.Clear(); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new ForgeTypeId_Clear();


        protected override bool CanBeSnoooped(Document document, ForgeTypeId value) => false;
        protected override string GetLabel(Document document, ForgeTypeId value) => "'not this time' - Benedict";
    }
}
