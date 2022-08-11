using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal class Group : QueryItem
    {
        public List<QueryItem> Elements { get; init; } = new List<QueryItem> ();


    }
}
