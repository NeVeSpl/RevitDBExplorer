using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class TableData_GetSectionData : MemberAccessorByType<TableData>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(TableData.GetSectionData);
        public override string MemberParams => typeof(SectionType).Name;
        IMemberAccessor IHaveFactoryMethod.Create() => new TableData_GetSectionData();


        protected override bool CanBeSnoooped(Document document, TableData tableData)
        {
            bool canBesnooped = tableData is not null && tableData.NumberOfSections > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, TableData tableData) => "[TableSectionData]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, TableData tableData)
        {
            foreach (SectionType type in Enum.GetValues(typeof(SectionType)))
            {
                var sectionData = tableData.GetSectionData(type);
                if (sectionData is null) continue;
                yield return new SnoopableObject(sectionData, document, type.ToString());
            }
        }
    }
}
