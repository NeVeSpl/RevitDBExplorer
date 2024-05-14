// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Members.Base
{
    internal enum MemberKind 
    {
        Property,
        Method,
        StaticMethod, 
        Extra,
        AsArgument,
        None
    }
}