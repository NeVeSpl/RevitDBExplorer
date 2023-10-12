using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels.Base
{
    internal interface ICanRead
    {
        void Read(SnoopableContext context, object @object);
    }


    internal interface ICanWrite
    {
        void Setup(Action raiseSnoopableObjectChanged);
    }


    internal interface ICanSnoop
    {
        bool CanBeSnooped
        {
            get;
        }

        IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object);
    }
}