using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetEntity : MemberAccessorByType<Element>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(Element.GetEntity);
        public override string MemberParams => typeof(Schema).Name;
        IMemberAccessor IHaveFactoryMethod.Create() => new Element_GetEntity();       



        protected override bool CanBeSnoooped(Document document, Element element)
        {
            foreach (var id in element.GetEntitySchemaGuids())
            {
                var schema = Schema.Lookup(id);
                if(schema.ReadAccessGranted()) return true;
            }
            return false;
        }
        protected override string GetLabel(Document document, Element element)
        {          
            return "[Entity]";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var schemas = Schema.ListSchemas();

            foreach (var schema in schemas)
            {
                if (!schema.ReadAccessGranted()) continue;
                var entity = element.GetEntity(schema);
                if (!entity.IsValid()) continue;

                yield return new SnoopableObject(schema, document, null, new[] { new SnoopableObject(entity, document) });
            }         
        }       
    }
}