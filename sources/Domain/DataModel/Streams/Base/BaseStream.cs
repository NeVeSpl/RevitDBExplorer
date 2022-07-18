using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Streams.Base
{
    internal abstract class BaseStream
    {
        public abstract IEnumerable<SnoopableMember> Stream(SnoopableObject snoopableObject);
        public virtual bool ShouldEndAllStreaming() => false;
    }
}