using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ExternalService_GetServer : MemberAccessorByType<ExternalService>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (ExternalService x) => x.GetServer(new Guid()); }


        protected override bool CanBeSnoooped(Document document, ExternalService externalService) => externalService.GetRegisteredServerIds().Any();
        protected override string GetLabel(Document document, ExternalService externalService) => Labeler.GetLabelForCollection(nameof(ExternalService), externalService.GetRegisteredServerIds().Count);
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ExternalService externalService)
        {
            foreach (var serverId in externalService.GetRegisteredServerIds())
            {
                yield return new SnoopableObject(document, externalService.GetServer(serverId));
            }
        }
    }
}
