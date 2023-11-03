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
    internal interface IAmSourceOfEverythingWithInfo : IAmSourceOfEverything
    {
        InfoAboutSource Info { get; }
    }

    internal sealed class SourceOfObjects
    {        
        private readonly IAmSourceOfEverything source;
        private InfoAboutSource info;
                
        public InfoAboutSource Info { get => info; init => info = value; }
        public IList<SnoopableObject> Objects { get; private set; } = new SnoopableObject[0];


        public SourceOfObjects()
        {
            
        }
        public SourceOfObjects(IAmSourceOfEverything source)
        {
            this.source = source;
        }
        public SourceOfObjects(IList<SnoopableObject> snoopableObjects)
        {
            Objects = snoopableObjects ?? new SnoopableObject[0];
        }


        public void ReadFromTheSource(UIApplication uiApplication)
        {
            if (source is null) return;
            
            var result = source.Snoop(uiApplication) ?? Enumerable.Empty<SnoopableObject>();
            Objects = result.ToArray();

            if (source is IAmSourceOfEverythingWithInfo sourceWithInfo) 
            {
                info = sourceWithInfo.Info;
            }            
        }
    }

    internal sealed class InfoAboutSource
    {
        public string ShortTitle { get; set; } = "";
        public string FullTitle { get; set; } = "";


        public InfoAboutSource()
        {
            
        }

        public InfoAboutSource(string shortTitle, string fullTitle = null)
        {
            ShortTitle = shortTitle;           
            FullTitle = fullTitle;
        }
    }
}