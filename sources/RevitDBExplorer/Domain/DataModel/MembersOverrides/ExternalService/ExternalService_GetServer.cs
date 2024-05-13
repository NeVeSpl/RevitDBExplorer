using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB.ExternalService;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class ExternalService_GetServer : MemberAccessorByType<ExternalService>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (ExternalService x) => x.GetServer(new Guid()) ];


        protected override ReadResult Read(SnoopableContext context, ExternalService externalService) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(ExternalService), externalService.GetRegisteredServerIds().Count),
            CanBeSnooped = externalService.GetRegisteredServerIds().Any()
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, ExternalService externalService)
        {
            foreach (var serverId in externalService.GetRegisteredServerIds())
            {
                yield return new SnoopableObject(context.Document, externalService.GetServer(serverId));
            }
        }
    }
}
