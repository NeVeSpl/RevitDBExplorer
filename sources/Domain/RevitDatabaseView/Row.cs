using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseView
{
    internal class Row
    {
        private readonly Element element;
        private readonly Dictionary<long, ICell> cells = new Dictionary<long, ICell>();


        public string Name => element.Name;

        public ICell this[long id]
        {
            get 
            { 
                cells.TryGetValue(id, out ICell cell);
                return cell;
            }         
        }


        public Row(Element element)
        {
            this.element = element;
        }
        public void AddCell(long id, ICell cell) 
        {
            cells.Add(id, cell);
        }

        public Parameter get_Parameter(Definition definition)
        {
            return element.get_Parameter(definition);
        }
    }
}