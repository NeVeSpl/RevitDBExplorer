using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure.StructuralSections;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors
{
    internal class StructuralSectionUtils_GetStructuralElementDefinitionData : MemberAccessorTypedWithDefaultPresenter<Element>
    {
        protected override ReadResult Read(SnoopableContext context, Element typedObject)
        {
            var status = StructuralSectionUtils.GetStructuralElementDefinitionData(context.Document, typedObject.Id, out var structuralElementDefinitionData);

            if (status == StructuralSectionErrorCode.Success )
            {
                return new ReadResult()
                {
                    CanBeSnooped = structuralElementDefinitionData != null,
                    Label = Labeler.GetLabelForObject(structuralElementDefinitionData, context),
                    AccessorName = nameof(StructuralSectionUtils_GetStructuralElementDefinitionData),                    
                };
            }
            return new ReadResult()
            {
                CanBeSnooped = false,
                Label = status.ToString(),
                AccessorName = nameof(StructuralSectionUtils_GetStructuralElementDefinitionData),
            };
        }


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Element typedObject, IValueContainer state)
        {
            var status = StructuralSectionUtils.GetStructuralElementDefinitionData(context.Document, typedObject.Id, out var structuralElementDefinitionData);
            yield return new SnoopableObject(context.Document, structuralElementDefinitionData);
        }
    }
}
