using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Document_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Document>.Create((doc, target) => BasicFileInfo.Extract(target.PathName), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => BasePoint.GetSurveyPoint(doc), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => BasePoint.GetProjectBasePoint(doc), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => InternalOrigin.Get(doc), kind: MemberKind.StaticMethod),

            MemberTemplate<Document>.Create((doc, target) => GlobalParametersManager.AreGlobalParametersAllowed(doc)),
            MemberTemplate<Document>.Create((doc, target) => GlobalParametersManager.GetAllGlobalParameters(doc)),
            MemberTemplate<Document>.Create((doc, target) => GlobalParametersManager.GetGlobalParametersOrdered(doc)),

            MemberTemplate<Document>.Create((doc, target) => LightGroupManager.GetLightGroupManager(doc)),
#if R2022_MIN
            MemberTemplate<Document>.Create((doc, target) => TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)),
#endif
#if R2023_MIN
            MemberTemplate<Document>.Create((doc, target) => AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc)),
#endif
            MemberTemplate<Document>.Create((doc, target) => StructuralSettings.GetStructuralSettings(doc)),
            MemberTemplate<Document>.Create((doc, target) => SunAndShadowSettings.GetActiveSunAndShadowSettings(target)),
#if R2022_MIN
            MemberTemplate<Document>.Create((doc, target) => RevisionNumberingSequence.GetAllRevisionNumberingSequences(target)),
#endif
            MemberTemplate<Document>.Create((doc, target) => AreaVolumeSettings.GetAreaVolumeSettings(target)),

#if R2025_MIN
            MemberTemplate<Document>.Create((doc, target) => Toposolid.IsSmoothedSurfaceEnabled(target)),
            MemberTemplate<Document>.Create((doc, target) => RebarSpliceTypeUtils.GetAllRebarSpliceTypes(target)),
            MemberTemplate<Document>.Create((doc, target) => LinearArray.GetMinimumSize(target)),
            MemberTemplate<Document>.Create((doc, target) => RadialArray.GetMinimumSize(target)),
            MemberTemplate<Document>.Create((doc, target) => IFCCategoryTemplate.GetActiveTemplate(target)),
            MemberTemplate<Document>.Create((doc, target) => IFCCategoryTemplate.ListNames(target)),
#endif

#if R2026_MIN
            MemberTemplate<Document>.Create((doc, target) => EnergyDataSettings.GetEnergyDataSettings(target)),
            MemberTemplate<Document>.Create((doc, target) => CableSize.GetCableSizeIds(target)),           
            MemberTemplate<Document>.CreateWithParam((doc, target, cableSizeId) => CableSize.GetCableSize(target, cableSizeId),  (doc, target) => CableSize.GetCableSizeIds(target)),
            MemberTemplate<Document>.Create((doc, target) => CableType.GetCableTypeIds(target)),
            MemberTemplate<Document>.Create((doc, target) => ConductorMaterial.GetConductorMaterialIds(target)),
            MemberTemplate<Document>.CreateWithParam((doc, target, conductorMaterialId) => ConductorMaterial.GetConductorMaterial(target, conductorMaterialId),  (doc, target) => ConductorMaterial.GetConductorMaterialIds(target)),
            MemberTemplate<Document>.Create((doc, target) => TemperatureRating.GetTemperatureRatingIds(target)),
            MemberTemplate<Document>.CreateWithParam((doc, target, temperatureRatingId) => TemperatureRating.GetTemperatureRating(target, temperatureRatingId),  (doc, target) => TemperatureRating.GetTemperatureRatingIds(target)),
            MemberTemplate<Document>.Create((doc, target) => InsulationMaterial.GetInsulationMaterialIds(target)),
            MemberTemplate<Document>.CreateWithParam((doc, target, insulationMaterialId) => InsulationMaterial.GetInsulationMaterial(target, insulationMaterialId),  (doc, target) => InsulationMaterial.GetInsulationMaterialIds(target)),
            MemberTemplate<Document>.Create((doc, target) => ConductorSize.GetConductorSizeIds(target)),
            MemberTemplate<Document>.CreateWithParam((doc, target, conductorSizeId) => ConductorSize.GetConductorSize(target, conductorSizeId),  (doc, target) => ConductorSize.GetConductorSizeIds(target)),

            MemberTemplate<Document>.Create((doc, target) => RebarCrankTypeUtils.GetAllRebarCrankTypes(target)),            
            MemberTemplate<Document>.Create((doc, target) => Toposolid.IsCutVoidStabilityEnabled(target)),            
#endif
        ];
    }
}