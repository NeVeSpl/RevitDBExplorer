using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal class ForgeTypeIdStream : BaseStream
    {
#if R2022 || R2023
        private static readonly HashSet<ForgeTypeId> AllDisciplines = new(UnitUtils.GetAllDisciplines());
#endif
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForForgeTypeId = Enumerable.Empty<ISnoopableMemberTemplate>();
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForCategory = Enumerable.Empty<ISnoopableMemberTemplate>();
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForParameter = Enumerable.Empty<ISnoopableMemberTemplate>();

        static ForgeTypeIdStream()
        {
            ForForgeTypeId = new ISnoopableMemberTemplate[]
                {
#if R2022 || R2023
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => Category.IsBuiltInCategory(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, BuiltInCategory>((doc, forgeId) => Category.GetBuiltInCategory(forgeId), x => Category.IsBuiltInCategory(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, StorageType>((doc, forgeId) => doc.GetTypeOfStorage(forgeId), x=> ParameterUtils.IsBuiltInParameter(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInParameter(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, BuiltInParameter>((doc, forgeId) => ParameterUtils.GetBuiltInParameter(forgeId), x=> ParameterUtils.IsBuiltInParameter(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInGroup(forgeId), x => true),
                    new SnoopableMemberTemplate<ForgeTypeId, BuiltInParameterGroup>((doc, forgeId) => ParameterUtils.GetBuiltInParameterGroup(forgeId), x=> ParameterUtils.IsBuiltInGroup(x)),

                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsMeasurableSpec(forgeId)),
#endif
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsSymbol(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsUnit(forgeId)),
#if R2022 || R2023
                    new SnoopableMemberTemplate<ForgeTypeId, ForgeTypeId>((doc, forgeId) => UnitUtils.GetDiscipline(forgeId), x => UnitUtils.IsMeasurableSpec(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForSpec(forgeId), x => UnitUtils.IsMeasurableSpec(x)),
#endif
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForUnit(forgeId), x => UnitUtils.IsUnit(x)),
#if R2022 || R2023
                    new SnoopableMemberTemplate<ForgeTypeId, IList<ForgeTypeId>>((doc, forgeId) => UnitUtils.GetValidUnits(forgeId), x => UnitUtils.IsMeasurableSpec(x)),


                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsValidDataType(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsSpec(forgeId)),

                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForBuiltInParameter(forgeId), x => ParameterUtils.IsBuiltInParameter(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForGroup(forgeId), x => ParameterUtils.IsBuiltInGroup(x)),
#endif
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForUnit(forgeId), x => UnitUtils.IsUnit(x)),
#if R2022 || R2023
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForSpec(forgeId), x => SpecUtils.IsSpec(x)),
#endif
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForSymbol(forgeId), x => UnitUtils.IsSymbol(x)),
#if R2022 || R2023
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForDiscipline(forgeId), x => AllDisciplines.Contains(x)),
#endif

                    new SnoopableMemberTemplate<ForgeTypeId, IList<ForgeTypeId>>((doc, forgeId) => FormatOptions.GetValidSymbols(forgeId), x => UnitUtils.IsUnit(x)),
                    new SnoopableMemberTemplate<ForgeTypeId,bool>((doc, forgeId) => FormatOptions.CanHaveSymbol(forgeId), x => UnitUtils.IsUnit(x)),
                };
            ForCategory = new ISnoopableMemberTemplate[]
                {
#if R2022 || R2023
                    new SnoopableMemberTemplate<Category, ForgeTypeId>((doc, category) => Category.GetBuiltInCategoryTypeId((BuiltInCategory)category.Id.IntegerValue)),
#endif
                };
            ForParameter = new ISnoopableMemberTemplate[]
                {
#if R2022 || R2023
                    new SnoopableMemberTemplate<Parameter, string>((doc, parameter) => UnitFormatUtils.Format(doc.GetUnits(), parameter.Definition.GetDataType(), parameter.AsDouble(), false), x => UnitUtils.IsMeasurableSpec(x.Definition?.GetDataType())),
#endif
                };            
        }


        public ForgeTypeIdStream()
        {
            RegisterTemplates(typeof(ForgeTypeId), ForForgeTypeId);
            RegisterTemplates(typeof(Category), ForCategory);
            RegisterTemplates(typeof(Parameter), ForParameter);
        }
    }
}