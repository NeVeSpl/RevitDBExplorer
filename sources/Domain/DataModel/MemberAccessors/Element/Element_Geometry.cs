using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal sealed class Element_Geometry : IMemberAccessor, IHaveFactoryMethod
    {    
        string IHaveFactoryMethod.TypeAndMemberName => "Element.Geometry";
        IMemberAccessor IHaveFactoryMethod.Create()
        {
            return new Element_Geometry();
        }


      
     
        public ReadResult Read(Document document, object @object)
        {
            var element = @object as Element;

            var options = element.ViewSpecific ? new Options() { View = document.ActiveView } : new Options();
            var geometry = element.get_Geometry(options);
            var canBeSnooped = geometry != null;

            return new ReadResult("[GeometryElement]", "GeometryElement", canBeSnooped);
        }
     
        public IEnumerable<SnoopableObject> Snooop(Document document, object @object)
        {
            var element = @object as Element;            

            var options = new List<Options>();
            if (document.ActiveView != null)
            {
                options.Add(new Options() { View = document.ActiveView, IncludeNonVisibleObjects = true });
                options.Add(new Options() { View = document.ActiveView, IncludeNonVisibleObjects = false });
            }

            foreach (ViewDetailLevel level in Enum.GetValues(typeof(ViewDetailLevel)))
            {
                options.Add(new Options() { DetailLevel = level, IncludeNonVisibleObjects = true });
                options.Add(new Options() { DetailLevel = level, IncludeNonVisibleObjects = false });
            }



            foreach (var option in options)
            {
                var result = element.get_Geometry(option);
                var viewName = option.View?.Name ?? "null";

                var snoopableObject = new SnoopableObject(result, document, $"{viewName}, {option.DetailLevel}" + (option.IncludeNonVisibleObjects ? ", include non-visible objects" : ""));
                yield return snoopableObject;
            }
        }
    }
}