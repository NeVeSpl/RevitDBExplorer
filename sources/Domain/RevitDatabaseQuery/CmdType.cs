// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal enum CmdType
    {
        ActiveView,       
        ElementId,
        ElementType,
        NotElementType,
        Category,
        Class,
        NameParam, 
        Parameter,
        StructuralType,
        Level,
        Room,
        RuleBasedFilter,
        Workset,
        Owned,
        Incorrect = 383,      
    }
}