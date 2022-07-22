using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal sealed class Element_Geometry : MemberAccessorByType<Element>, IHaveFactoryMethod
    {          
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Element x, Options o) => x.get_Geometry(o); } }
        IMemberAccessor IHaveFactoryMethod.Create()
        {
            return new Element_Geometry();
        }


        protected override bool CanBeSnoooped(Document document, Element element)
        {          
            var options = element.ViewSpecific ? new Options() { View = document.ActiveView } : new Options();
            var geometry = element.get_Geometry(options);
            var canBeSnooped = geometry != null;

            return canBeSnooped;
        }
        protected override string GetLabel(Document document, Element element) => $"[{nameof(GeometryElement)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var optionsForActiveView = new List<Options>();
            if (document.ActiveView != null)
            {
                optionsForActiveView.Add(new Options() { View = document.ActiveView, IncludeNonVisibleObjects = false, ComputeReferences = true });
                optionsForActiveView.Add(new Options() { View = document.ActiveView, IncludeNonVisibleObjects = true, ComputeReferences = true });
                yield return new SnoopableObject(document, null, GetGeometry(document, element, optionsForActiveView)) { Name = "Active view: " + document.ActiveView.Name, NamePrefix="view:" };
            }

            var options = new List<Options>();
            foreach (ViewDetailLevel level in Enum.GetValues(typeof(ViewDetailLevel)))
            {
                options.Add(new Options() { DetailLevel = level, IncludeNonVisibleObjects = false, ComputeReferences=true });
                options.Add(new Options() { DetailLevel = level, IncludeNonVisibleObjects = true, ComputeReferences = true });
            }
            yield return new SnoopableObject(document, null, GetGeometry(document, element, options)) { Name = "null", NamePrefix = "view:" };
        }

        private IEnumerable<SnoopableObject> GetGeometry(Document document, Element element, IEnumerable<Options> options)
        {
            foreach (var option in options)
            {
                var result = element.get_Geometry(option);
                var snoopableObject = new SnoopableObject(document, result) { Name = $"{option.DetailLevel}" + (option.IncludeNonVisibleObjects ? ", include non-visible objects" : ""), NamePrefix= "detail level:"  };
                yield return snoopableObject;
            }
        }
    }
}