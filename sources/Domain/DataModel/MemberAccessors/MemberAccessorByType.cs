using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal abstract class MemberAccessorByType<TSnoopedObjectType> : MemberAccessorTyped<TSnoopedObjectType> where TSnoopedObjectType : class
    {
        protected abstract IEnumerable<LambdaExpression> HandledMembers { get; }
        public virtual IEnumerable<string> GetHandledMembers() => HandledMembers.Select(x => x.GetUniqueId());
            

        public override ReadResult Read(Document document, TSnoopedObjectType @object)
        {           
            string label = GetLabel(document, @object);
            bool canBeSnooped = CanBeSnoooped(document, @object);

            return new ReadResult(label, GetHandledMembers().FirstOrDefault(), canBeSnooped);
        }
        protected abstract bool CanBeSnoooped(Document document, TSnoopedObjectType value);
        protected abstract string GetLabel(Document document, TSnoopedObjectType value);

        public override IEnumerable<SnoopableObject> Snoop(Document document, TSnoopedObjectType @object)
        {
            return this.Snooop(document, @object);
        }
        protected virtual IEnumerable<SnoopableObject> Snooop(Document document, TSnoopedObjectType value) => Enumerable.Empty<SnoopableObject>();       
    }
}