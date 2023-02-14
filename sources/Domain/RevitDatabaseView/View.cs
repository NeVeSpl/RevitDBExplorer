using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseView
{
    internal class View
    {
        List<Column> columns;
        List<Row> rows;

        public IEnumerable<Column> Columns => columns;
        public IEnumerable<Row> Rows => rows;


        private View(IList<Element> elements)
        {
            var categories = elements.Select(x => x.Category).Distinct().ToArray();           

            var columnsMap = new Dictionary<ElementId, Column>();
            rows = new List<Row>(elements.Count);

            foreach (var element in elements)
            {
                var row = new Row(element);

                foreach (Parameter parameter in element.Parameters)
                {
                    var column = columnsMap.GetOrCreate(parameter.Id, x => new Column(element.Document, parameter));

                    var cell = new Cell(row, column);
                    row.AddCell(parameter.Id.Value(), cell);
                    cell.Read();
                }

                rows.Add(row);
            }

            columns = columnsMap.Values.OrderBy(x => x.Name).ToList();
        }





        public static View Create(Document document, IList<Element> elements)
        {
            return new View(elements);
        }
    }
}
