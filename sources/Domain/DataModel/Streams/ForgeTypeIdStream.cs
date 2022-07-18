using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal class ForgeTypeIdStream : BaseStream
    {
        private static readonly HashSet<ForgeTypeId> AllDisciplines = new HashSet<ForgeTypeId>(UnitUtils.GetAllDisciplines());
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForForgeTypeId = Enumerable.Empty<ISnoopableMemberTemplate>();
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForCategory = Enumerable.Empty<ISnoopableMemberTemplate>();
        private static readonly IEnumerable<ISnoopableMemberTemplate> ForParameter = Enumerable.Empty<ISnoopableMemberTemplate>();

        static ForgeTypeIdStream()
        {
            ForForgeTypeId = new ISnoopableMemberTemplate[]
                {
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => Category.IsBuiltInCategory(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, BuiltInCategory>((doc, forgeId) => Category.GetBuiltInCategory(forgeId), x => Category.IsBuiltInCategory(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, StorageType>((doc, forgeId) => doc.GetTypeOfStorage(forgeId), x=> ParameterUtils.IsBuiltInParameter(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInParameter(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, BuiltInParameter>((doc, forgeId) => ParameterUtils.GetBuiltInParameter(forgeId), x=> ParameterUtils.IsBuiltInParameter(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInGroup(forgeId), x => true),
                    new SnoopableMemberTemplate<ForgeTypeId, BuiltInParameterGroup>((doc, forgeId) => ParameterUtils.GetBuiltInParameterGroup(forgeId), x=> ParameterUtils.IsBuiltInGroup(x)),

                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsMeasurableSpec(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsSymbol(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsUnit(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, ForgeTypeId>((doc, forgeId) => UnitUtils.GetDiscipline(forgeId), x => UnitUtils.IsMeasurableSpec(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForSpec(forgeId), x => UnitUtils.IsMeasurableSpec(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForUnit(forgeId), x => UnitUtils.IsUnit(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, IList<ForgeTypeId>>((doc, forgeId) => UnitUtils.GetValidUnits(forgeId), x => UnitUtils.IsMeasurableSpec(x)),

                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsValidDataType(forgeId)),
                    new SnoopableMemberTemplate<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsSpec(forgeId)),

                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForBuiltInParameter(forgeId), x => ParameterUtils.IsBuiltInParameter(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForGroup(forgeId), x => ParameterUtils.IsBuiltInGroup(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForUnit(forgeId), x => UnitUtils.IsUnit(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForSpec(forgeId), x => SpecUtils.IsSpec(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForSymbol(forgeId), x => UnitUtils.IsSymbol(x)),
                    new SnoopableMemberTemplate<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForDiscipline(forgeId), x => AllDisciplines.Contains(x)),

                    new SnoopableMemberTemplate<ForgeTypeId, IList<ForgeTypeId>>((doc, forgeId) => FormatOptions.GetValidSymbols(forgeId), x => UnitUtils.IsUnit(x)),
                    new SnoopableMemberTemplate<ForgeTypeId,bool>((doc, forgeId) => FormatOptions.CanHaveSymbol(forgeId), x => UnitUtils.IsUnit(x)),
                };
            ForCategory = new ISnoopableMemberTemplate[]
                {
                    new SnoopableMemberTemplate<Category, ForgeTypeId>((doc, category) => Category.GetBuiltInCategoryTypeId(category.BuiltInCategory)),
                };
            ForParameter = new ISnoopableMemberTemplate[]
                {
                    new SnoopableMemberTemplate<Parameter, string>((doc, parameter) => UnitFormatUtils.Format(doc.GetUnits(), parameter.Definition.GetDataType(), parameter.AsDouble(), false)),
                };
        }

        public override IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject)
        {            
            if (snoopableObject.Object is ForgeTypeId id)
            {
                foreach (var item in ForForgeTypeId)
                {
                    if (item.ShouldBeCreated(id))
                    {
                        var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.StaticMethod, item.MemberName, item.DeclaringType, item.MemberAccessor, null);
                        yield return member;
                    }
                }
            }
            if (snoopableObject.Object is Category _)
            {
                foreach (var item in ForCategory)
                {
                    var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.StaticMethod, item.MemberName, item.DeclaringType, item.MemberAccessor, null);
                    yield return member;
                }
            }
            if (snoopableObject.Object is Parameter par)
            {
                foreach (var item in ForParameter)
                {
                    var dataType = par.Definition?.GetDataType();
                    if (UnitUtils.IsMeasurableSpec(dataType))
                    {
                        var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.StaticMethod, item.MemberName, item.DeclaringType, item.MemberAccessor, null);
                        yield return member;
                    }
                }
            }
        }
    }
}