using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class ForgeTypeId_Templates : IHaveMemberTemplates
    {
#if R2022b
        private static readonly HashSet<ForgeTypeId> AllDisciplines = new(UnitUtils.GetAllDisciplines());
#endif
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForForgeTypeId = Enumerable.Empty<ISnoopableMemberTemplate>();
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForCategory = Enumerable.Empty<ISnoopableMemberTemplate>();
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForParameter = Enumerable.Empty<ISnoopableMemberTemplate>();


        static ForgeTypeId_Templates()
        {
            ForForgeTypeId = new ISnoopableMemberTemplate[]
            {
#if R2022b
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => Category.IsBuiltInCategory(forgeId)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => Category.GetBuiltInCategory(forgeId), x => Category.IsBuiltInCategory(x)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => doc.GetTypeOfStorage(forgeId), x=> ParameterUtils.IsBuiltInParameter(x)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => ParameterUtils.IsBuiltInParameter(forgeId)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => ParameterUtils.GetBuiltInParameter(forgeId), x=> ParameterUtils.IsBuiltInParameter(x)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => ParameterUtils.IsBuiltInGroup(forgeId), x => true),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => ParameterUtils.GetBuiltInParameterGroup(forgeId), x=> ParameterUtils.IsBuiltInGroup(x)),

                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => UnitUtils.IsMeasurableSpec(forgeId)),
#endif
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => UnitUtils.IsSymbol(forgeId)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => UnitUtils.IsUnit(forgeId)),
#if R2022b
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => UnitUtils.GetDiscipline(forgeId), x => UnitUtils.IsMeasurableSpec(x)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => UnitUtils.GetTypeCatalogStringForSpec(forgeId), x => UnitUtils.IsMeasurableSpec(x)),
#endif
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => UnitUtils.GetTypeCatalogStringForUnit(forgeId), x => UnitUtils.IsUnit(x)),
#if R2022b
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => UnitUtils.GetValidUnits(forgeId), x => UnitUtils.IsMeasurableSpec(x)),


                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => SpecUtils.IsValidDataType(forgeId)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => SpecUtils.IsSpec(forgeId)),

                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => LabelUtils.GetLabelForBuiltInParameter(forgeId), x => ParameterUtils.IsBuiltInParameter(x)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => LabelUtils.GetLabelForGroup(forgeId), x => ParameterUtils.IsBuiltInGroup(x)),
#endif
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => LabelUtils.GetLabelForUnit(forgeId), x => UnitUtils.IsUnit(x)),
#if R2022b
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => LabelUtils.GetLabelForSpec(forgeId), x => SpecUtils.IsSpec(x)),
#endif
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => LabelUtils.GetLabelForSymbol(forgeId), x => UnitUtils.IsSymbol(x)),
#if R2022b
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => LabelUtils.GetLabelForDiscipline(forgeId), x => AllDisciplines.Contains(x)),
#endif

                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => FormatOptions.GetValidSymbols(forgeId), x => UnitUtils.IsUnit(x)),
                SnoopableMemberTemplate<ForgeTypeId>.Create((doc, forgeId) => FormatOptions.CanHaveSymbol(forgeId), x => UnitUtils.IsUnit(x)),
            };
            ForCategory = new ISnoopableMemberTemplate[]
            {
#if R2022b
                SnoopableMemberTemplate<Category>.Create((doc, category) => Category.GetBuiltInCategoryTypeId((BuiltInCategory)category.Id.Value())),
#endif
            };
            ForParameter = new ISnoopableMemberTemplate[]
            {
#if R2022b
                SnoopableMemberTemplate<Parameter>.Create((doc, parameter) => UnitFormatUtils.Format(doc.GetUnits(), parameter.Definition.GetDataType(), parameter.AsDouble(), false), x => UnitUtils.IsMeasurableSpec(x.Definition?.GetDataType())),
#endif
            };            
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return ForForgeTypeId.Concat(ForCategory).Concat(ForParameter);
        }
    }
}