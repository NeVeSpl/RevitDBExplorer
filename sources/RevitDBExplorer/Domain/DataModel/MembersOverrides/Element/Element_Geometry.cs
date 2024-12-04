using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal sealed class Element_Geometry : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Element x, Options o) => x.get_Geometry(o) ];


        protected override ReadResult Read(SnoopableContext context, Element element) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(GeometryElement), null),
            CanBeSnooped = CanBeSnoooped(context.Document, element),
        };
        private static bool CanBeSnoooped(Document document, Element element)
        {
            if (element.ViewSpecific)
            {
                var options = GetOptionsForView(document.ActiveView);
                var geometry = element.get_Geometry(options);
                var canBeSnooped = geometry != null;
                return canBeSnooped;
            } 
            else
            { 
                var geometry = element.get_Geometry(new Options());
                var geometryViewSpecific = element.get_Geometry(GetOptionsForView(document.ActiveView));
                var canBeSnooped = geometry != null || geometryViewSpecific != null;
                return canBeSnooped;
            }
        }
        

        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Element element)
        {
            var document = context.Document;
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

        private static IEnumerable<SnoopableObject> GetGeometry(Document document, Element element, IEnumerable<Options> options)
        {
            foreach (var option in options)
            {
                var result = element.get_Geometry(option);
                var snoopableObject = new SnoopableObject(document, result) { Name = $"{option.DetailLevel}" + (option.IncludeNonVisibleObjects ? ", include non-visible objects" : ""), NamePrefix= "detail level:"  };
                yield return snoopableObject;
            }
        }

        private static Options GetOptionsForView(View view)
        {
            if (view != null)
            {
                return new Options() { View = view };
            }
            return new Options();
        }
    }
}