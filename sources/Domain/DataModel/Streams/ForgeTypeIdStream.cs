using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams
{
    internal class ForgeTypeIdStream
    {
        private static readonly StaticMember[] ForForgeTypeId = new StaticMember[0];
        private static readonly StaticMember[] ForCategory = new StaticMember[0];
        private static readonly StaticMember[] ForParameter = new StaticMember[0];

        static ForgeTypeIdStream()
        {
            ForForgeTypeId = new StaticMember[]
                {
                    new (typeof(Category), nameof(Category.IsBuiltInCategory), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => Category.IsBuiltInCategory(forgeId))),
                    new (typeof(Category), nameof(Category.GetBuiltInCategory), new MemberAccessorByFunc<ForgeTypeId, BuiltInCategory>((doc, forgeId) => Category.GetBuiltInCategory(forgeId))),
                    new (typeof(Document), nameof(Document.GetTypeOfStorage), new MemberAccessorByFunc<ForgeTypeId, StorageType>((doc, forgeId) => doc.GetTypeOfStorage(forgeId))),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.IsBuiltInParameter), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInParameter(forgeId))),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.GetBuiltInParameter), new MemberAccessorByFunc<ForgeTypeId, BuiltInParameter>((doc, forgeId) => ParameterUtils.GetBuiltInParameter(forgeId))),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.IsBuiltInGroup), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => ParameterUtils.IsBuiltInGroup(forgeId))),
                    new (typeof(ParameterUtils), nameof(ParameterUtils.GetBuiltInParameterGroup), new MemberAccessorByFunc<ForgeTypeId, BuiltInParameterGroup>((doc, forgeId) => ParameterUtils.GetBuiltInParameterGroup(forgeId))),

                    new (typeof(UnitUtils), nameof(UnitUtils.IsMeasurableSpec), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsMeasurableSpec(forgeId))),
                    new (typeof(UnitUtils), nameof(UnitUtils.IsSymbol), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsSymbol(forgeId))),
                    new (typeof(UnitUtils), nameof(UnitUtils.IsUnit), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => UnitUtils.IsUnit(forgeId))),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetDiscipline), new MemberAccessorByFunc<ForgeTypeId, ForgeTypeId>((doc, forgeId) => UnitUtils.GetDiscipline(forgeId))),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetTypeCatalogStringForSpec), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForSpec(forgeId))),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetTypeCatalogStringForUnit), new MemberAccessorByFunc<ForgeTypeId, string>((doc, forgeId) => UnitUtils.GetTypeCatalogStringForUnit(forgeId))),
                    new (typeof(UnitUtils), nameof(UnitUtils.GetValidUnits), new MemberAccessorByFunc<ForgeTypeId, IList<ForgeTypeId>>((doc, forgeId) => UnitUtils.GetValidUnits(forgeId))),

                    new (typeof(SpecUtils), nameof(SpecUtils.IsValidDataType), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsValidDataType(forgeId))),
                    new (typeof(SpecUtils), nameof(SpecUtils.IsSpec), new MemberAccessorByFunc<ForgeTypeId, bool>((doc, forgeId) => SpecUtils.IsSpec(forgeId))),
                };
            ForCategory = new StaticMember[]
                {
                    new (typeof(Category), nameof(Category.GetBuiltInCategoryTypeId), new MemberAccessorByFunc<Category, ForgeTypeId>((doc, category) => Category.GetBuiltInCategoryTypeId(category.BuiltInCategory))),
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
                    var member = new SnoopableMember(snoopableObject, SnoopableMember.Kind.StaticMethod, item.MemberName, item.DeclaringType, item.MemberAccessor, null);
                    yield return member;
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


        public record class StaticMember(Type DeclaringType, string MemberName, IMemberAccessor MemberAccessor);

    }
}