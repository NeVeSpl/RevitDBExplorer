using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class ClassMatch : LookupResult<Type>
    {
        public ClassMatch(Type value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.Class;
            Name = $"typeof({value.Name})";
            Label = value.Name;
        }
    }
}