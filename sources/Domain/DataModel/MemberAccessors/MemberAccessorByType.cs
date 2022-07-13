using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal abstract class MemberAccessorByType<T> : IMemberAccessor where T : class
    {
        protected abstract IEnumerable<LambdaExpression> HandledMembers { get; }
        public virtual IEnumerable<string> GetHandledMembers() => HandledMembers.Select(x => (x.Body as MethodCallExpression).Method.GetUniqueId());


        public ReadResult Read(Document document, object @object)
        {
            T target =  @object as T;
            string label = GetLabel(document, target);
            bool canBeSnooped = CanBeSnoooped(document, target);

            return new ReadResult(label, GetHandledMembers().First(), canBeSnooped);
        }
        protected abstract bool CanBeSnoooped(Document document, T value);
        protected abstract string GetLabel(Document document, T value);

        public IEnumerable<SnoopableObject> Snoop(Document document, object @object)
        {
            return this.Snooop(document, @object as T);
        }
        protected virtual IEnumerable<SnoopableObject> Snooop(Document document, T value) => Enumerable.Empty<SnoopableObject>();       
    }
}