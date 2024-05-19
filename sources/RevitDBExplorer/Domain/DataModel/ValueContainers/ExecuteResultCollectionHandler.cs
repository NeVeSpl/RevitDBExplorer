using System.Collections.Generic;
using System.Linq;
using System.Collections;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class ExecuteResultCollectionHandler : TypeHandler<ExecuteResultCollection>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, ExecuteResultCollection value)
        {
            return value.Results.Count > 0;
        }
        protected override string ToLabel(SnoopableContext context, ExecuteResultCollection value)
        {
            return value.Label;
        }

        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, ExecuteResultCollection value)
        {
            foreach (var pair in value.Results)
            {
                var subObjects = new List<SnoopableObject>();

                if (pair.Result is IEnumerable enumerable)
                {                  
                    foreach (var item in enumerable)
                    {
                        subObjects.Add(new SnoopableObject(context.Document, item));
                    }
                }
                else
                {
                    subObjects.Add(new SnoopableObject(context.Document, pair.Result));
                }

                yield return new SnoopableObject(context.Document, pair.Arg, subObjects) { NamePrefix = value.Param_0_Name + " = " } ;
            }
        }
    }
}