using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class LocationHandler : TypeHandler<Location>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Location location) => true;

        protected override string ToLabel(SnoopableContext context, Location location)
        {
            string typeName = location.GetType()?.GetCSharpName();

            string details = "";

            switch (location)
            {
                case LocationPoint locationPoint:
                    details = $"({locationPoint.Point.X:f2}, {locationPoint.Point.Y:f2}, {locationPoint.Point.Z:f2})";
                    break;
                case LocationCurve locationCurve:
                    if (locationCurve.Curve is not null)
                    {
                        var start = locationCurve.Curve.GetEndPoint(0);
                        var end = locationCurve.Curve.GetEndPoint(1);
                        details = $"({start.X:f2}, {start.Y:f2}, {start.Z:f2}) - ({end.X:f2}, {end.Y:f2}, {end.Z:f2})";
                    }
                    break;
            }


            return $"{typeName} : {details}";
        }        
    }
}