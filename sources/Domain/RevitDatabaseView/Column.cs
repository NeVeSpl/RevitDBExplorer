using Autodesk.Revit.DB;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseView
{
    internal class Column : BaseViewModel
    {
        private readonly Parameter parameter;
       
        private readonly Definition definition;


        public string Name => definition?.Name;
        public ElementId Id => parameter.Id;
        public Definition Definition => definition;
        

        public Column(Document document, Parameter parameter)      
        {
            this.parameter = parameter;
            this.definition = parameter.Definition;
        }
    }
}