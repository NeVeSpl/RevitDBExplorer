using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal class ForgeTypeIdStream
    {
        private static readonly HashSet<ForgeTypeId> AllDisciplines = new HashSet<ForgeTypeId>(UnitUtils.GetAllDisciplines());
        private static readonly StaticMember[] ForForgeTypeId = new StaticMember[0];
        private static readonly StaticMember[] ForCategory = new StaticMember[0];
        private static readonly StaticMember[] ForParameter = new StaticMember[0];

        static ForgeTypeIdStream()
        {
            ForForgeTypeId = new StaticMember[]
                {
                    new (typeof(Category), nameof(Category.IsBuiltInCategory), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => Category.IsBuiltInCategory(forgeId)), x => true),
                    new (typeof(Category), nameof(Category.GetBuiltInCategory), new MemberAccessorByFunc<ForgeTypeId, BuiltInCategory>((doc, forgeId) => Category.GetBuiltInCategory(forgeId)), x => Category.IsBuiltInCategory(x)),
                    new (typeof(Document), nameof(Document.GetTypeOfStorage), new MemberAccessorByFunc<ForgeTypeId, StorageType>((doc, forgeId) => doc.GetTypeOfStorage(forgeId)), x=> ParameterUtils.IsBuiltInParameter(x)),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.IsBuiltInParameter), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInParameter(forgeId)), x => true),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.GetBuiltInParameter), new MemberAccessorByFunc<ForgeTypeId, BuiltInParameter>((doc, forgeId) => ParameterUtils.GetBuiltInParameter(forgeId)), x=> ParameterUtils.IsBuiltInParameter(x)),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.IsBuiltInGroup), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInGroup(forgeId)), x => true),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.GetBuiltInParameterGroup), new MemberAccessorByFunc<ForgeTypeId, BuiltInParameterGroup>((doc, forgeId) => ParameterUtils.GetBuiltInParameterGroup(forgeId)), x=> ParameterUtils.IsBuiltInGroup(x)),

                    new (typeof(UnitUtils), nameof(UnitUtils.IsMeasurableSpec), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsMeasurableSpec(forgeId)), x => true),
                    new (typeof(UnitUtils), nameof(UnitUtils.IsSymbol), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsSymbol(forgeId)), x => true),
                    new (typeof(UnitUtils), nameof(UnitUtils.IsUnit), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsUnit(forgeId)), x => true),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetDiscipline), new MemberAccessorByFunc<ForgeTypeId, ForgeTypeId>((doc, forgeId) => UnitUtils.GetDiscipline(forgeId)), x => UnitUtils.IsMeasurableSpec(x)),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetTypeCatalogStringForSpec), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForSpec(forgeId)), x => UnitUtils.IsMeasurableSpec(x)),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetTypeCatalogStringForUnit), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForUnit(forgeId)), x => UnitUtils.IsUnit(x)),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetValidUnits), new MemberAccessorByFunc<ForgeTypeId, IList<ForgeTypeId>>((doc, forgeId) => UnitUtils.GetValidUnits(forgeId)), x => UnitUtils.IsMeasurableSpec(x)),

                    new (typeof(SpecUtils), nameof(SpecUtils.IsValidDataType), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsValidDataType(forgeId)), x => true),
                    new (typeof(SpecUtils), nameof(SpecUtils.IsSpec), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsSpec(forgeId)), x => true),

                    new (typeof(LabelUtils), nameof(LabelUtils.GetLabelForBuiltInParameter), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForBuiltInParameter(forgeId)), x => ParameterUtils.IsBuiltInParameter(x)),
                    new (typeof(LabelUtils), nameof(LabelUtils.GetLabelForGroup), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForGroup(forgeId)), x => ParameterUtils.IsBuiltInGroup(x)),
                    new (typeof(LabelUtils), nameof(LabelUtils.GetLabelForUnit), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForUnit(forgeId)), x => UnitUtils.IsUnit(x)),
                    new (typeof(LabelUtils), nameof(LabelUtils.GetLabelForSpec), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForSpec(forgeId)), x => SpecUtils.IsSpec(x)),
                    new (typeof(LabelUtils), nameof(LabelUtils.GetLabelForSymbol), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForSymbol(forgeId)), x => UnitUtils.IsSymbol(x)),
                    new (typeof(LabelUtils), nameof(LabelUtils.GetLabelForDiscipline), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => LabelUtils.GetLabelForDiscipline(forgeId)), x => AllDisciplines.Contains(x)),

                    new (typeof(FormatOptions), nameof(FormatOptions.GetValidSymbols), new MemberAccessorByFunc<ForgeTypeId, IList<ForgeTypeId>>((doc, forgeId) => FormatOptions.GetValidSymbols(forgeId)), x => UnitUtils.IsUnit(x)),
                    new (typeof(FormatOptions), nameof(FormatOptions.CanHaveSymbol), new MemberAccessorByFunc<ForgeTypeId,bool>((doc, forgeId) => FormatOptions.CanHaveSymbol(forgeId)), x => UnitUtils.IsUnit(x)),
                };
            ForCategory = new StaticMember[]
                {
                    new (typeof(Category), nameof(Category.GetBuiltInCategoryTypeId), new MemberAccessorByFunc<Category, ForgeTypeId>((doc, category) => Category.GetBuiltInCategoryTypeId(category.BuiltInCategory)), x => true),
                };
            ForParameter = new StaticMember[]
                {
                    //new (typeof(ParameterUtils), nameof(ParameterUtils.GetParameterTypeId), new MemberAccessorByFunc<Parameter, ForgeTypeId>((doc, parameter) => ParameterUtils.GetParameterTypeId(parameter))),
                };
        }

        public IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject)
        {            
            if (snoopableObject.Object is ForgeTypeId id)
            {

                foreach (var item in ForForgeTypeId)
                {
                    if (item.shouldBeDisplayed(id))
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
        }


        public record class StaticMember(Type DeclaringType, string MemberName, IMemberAccessor MemberAccessor, Func<ForgeTypeId, bool> shouldBeDisplayed);

    }
}