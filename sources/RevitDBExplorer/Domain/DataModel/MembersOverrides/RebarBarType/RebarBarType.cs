using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class RebarBarType_Overrides : IHaveMembersOverrides
    {
        public IEnumerable<IMemberOverride> GetOverrides() =>
        [
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, hookId) => target.GetAutoCalcHookLengths(hookId), (document, target) => new FilteredElementCollector(document).OfClass(typeof(RebarHookType)).ToElementIds()),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, hookId) => target.GetHookLength(hookId), (document, target) => new FilteredElementCollector(document).OfClass(typeof(RebarHookType)).ToElementIds()),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, hookId) => target.GetHookPermission(hookId), (document, target) => new FilteredElementCollector(document).OfClass(typeof(RebarHookType)).ToElementIds()),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, hookId) => target.GetHookTangentLength(hookId), (document, target) => new FilteredElementCollector(document).OfClass(typeof(RebarHookType)).ToElementIds()),


        #if R2025_MIN
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarSpliceTypeId) => target.GetLapLength(rebarSpliceTypeId), (document, target) => RebarSpliceTypeUtils.GetAllRebarSpliceTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarSpliceTypeId) => target.GetAutoCalculatedLapLength(rebarSpliceTypeId), (document, target) => RebarSpliceTypeUtils.GetAllRebarSpliceTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarSpliceTypeId) => target.GetStaggerLength(rebarSpliceTypeId), (document, target) => RebarSpliceTypeUtils.GetAllRebarSpliceTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarSpliceTypeId) => target.GetAutoCalculatedStaggerLength(rebarSpliceTypeId), (document, target) => RebarSpliceTypeUtils.GetAllRebarSpliceTypes(document)),
        #endif

        #if R2026_MIN
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarCrankTypeId) => target.GetAutoCalculatedCrank(rebarCrankTypeId), (document, target) => RebarCrankTypeUtils.GetAllRebarCrankTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarCrankTypeId) => target.GetCrankLength(rebarCrankTypeId), (document, target) => RebarCrankTypeUtils.GetAllRebarCrankTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarCrankTypeId) => target.GetCrankAngledLength(rebarCrankTypeId), (document, target) => RebarCrankTypeUtils.GetAllRebarCrankTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarCrankTypeId) => target.GetCrankOffsetLength(rebarCrankTypeId), (document, target) => RebarCrankTypeUtils.GetAllRebarCrankTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarCrankTypeId) => target.GetCrankRatio(rebarCrankTypeId), (document, target) => RebarCrankTypeUtils.GetAllRebarCrankTypes(document)),
            MemberOverride<RebarBarType>.ByFuncWithParam((document, target, rebarCrankTypeId) => target.GetCrankStraightLength(rebarCrankTypeId), (document, target) => RebarCrankTypeUtils.GetAllRebarCrankTypes(document)),
        #endif
        ];
    }
}