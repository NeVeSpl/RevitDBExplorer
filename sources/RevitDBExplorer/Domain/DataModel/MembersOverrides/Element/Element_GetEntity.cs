using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Element_GetEntity : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Element x, Schema s) => x.GetEntity(s) ];


        protected override ReadResult Read(SnoopableContext context, Element element) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Entity), null),
            CanBeSnooped = CanBeSnoooped(element)
        };
        private static bool CanBeSnoooped(Element element)
        {
            foreach (var id in element.GetEntitySchemaGuids())
            {
                var schema = Schema.Lookup(id);
                if(schema.ReadAccessGranted()) return true;
            }
            return false;
        }
      

        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Element element)
        {
            var schemas = Schema.ListSchemas();

            foreach (var schema in schemas)
            {
                if (!schema.ReadAccessGranted()) continue;
                var entity = element.GetEntity(schema);
                if (!entity.IsValid()) continue;

                yield return SnoopableObject.CreateInOutPair(context.Document, schema, entity);
            }         
        }       
    }
}