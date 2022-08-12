using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class StructuralTypeMatch : LookupResult<StructuralType>
    {
        public StructuralTypeMatch(StructuralType value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.StructuralType;
            Name = $"StructuralType.{value}";
        }
    }
}