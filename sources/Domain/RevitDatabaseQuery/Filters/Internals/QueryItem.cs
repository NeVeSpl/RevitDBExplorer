using System;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal abstract class QueryItem
    {       
        public string FilterSyntax
        {
            get; 
            init;
        } = String.Empty;
        public string CollectorSyntax
        {
            get 
            {
                if (!string.IsNullOrEmpty(FilterSyntax) && !FilterSyntax.StartsWith("."))
                {
                    return $".WherePasses({FilterSyntax})";
                }
                return FilterSyntax;
            }
        }


        public abstract ElementFilter CreateElementFilter(Document document);
    }
}