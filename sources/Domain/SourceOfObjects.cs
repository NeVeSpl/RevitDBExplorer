using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal interface IAmSourceOfEverything
    {
        IEnumerable<SnoopableObject> Snoop(UIApplication app);
    }


    internal sealed class SourceOfObjects
    {        
        private readonly IAmSourceOfEverything source;
        private IList<SnoopableObject> snoopableObjects;

        public string Title { get; init; }
        public IList<SnoopableObject> Objects  => snoopableObjects ?? new SnoopableObject[0];


        public SourceOfObjects()
        {

        }
        public SourceOfObjects(IAmSourceOfEverything source)
        {
            this.source = source;
        }
        public SourceOfObjects(IList<SnoopableObject> snoopableObjects)
        {
            this.snoopableObjects = snoopableObjects;
        }


        public void ReadFromTheSource(UIApplication uiApplication)
        {
            var result = source.Snoop(uiApplication) ?? Enumerable.Empty<SnoopableObject>();
            this.snoopableObjects = result.ToArray();
        }
    }
}