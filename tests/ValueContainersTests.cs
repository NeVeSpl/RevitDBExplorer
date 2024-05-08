using System;
using System.IO;
using Autodesk.Revit.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ValueContainers;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitTestLibrary;

namespace RevitDBExplorer.Tests
{
    [TestClass]
    public class ValueContainersTests
    {
        [RevitTestMethod]       
        public void SelectTypeHandlerFor_BuiltInParameter(RevitContext revitContext)
        {
            var type = typeof(BuiltInParameter);
            var typeHandler = ValueContainerFactory.SelectTypeHandlerFor(type);

            Assert.AreEqual(typeof(EnumHandler<BuiltInParameter>), typeHandler.GetType());
            Assert.AreEqual(typeof(BuiltInParameter), typeHandler.Type);
        }

        [RevitTestMethod]
        public void SelectTypeHandlerFor_Long(RevitContext revitContext)
        {
            var type = typeof(long);
            var typeHandler = ValueContainerFactory.SelectTypeHandlerFor(type);

            Assert.AreEqual(typeof(ValueTypeHandler<long>), typeHandler.GetType());
            Assert.AreEqual(typeof(long), typeHandler.Type);
        }


        [RevitTestMethod]
        public void CreateValueContainerFor_BuiltInParameter(RevitContext revitContext)
        {           
            var document = OpenRevitFile(revitContext);

            var type = typeof(BuiltInParameter);
            var valueContainer = ValueContainerFactory.Create(type);
            valueContainer.SetValue(new SnoopableContext() { Document = document }, BuiltInParameter.FUNCTION_PARAM);

            Assert.AreEqual(typeof(ValueContainer<System.Enum>), valueContainer.GetType());
            Assert.AreEqual(typeof(EnumHandler<System.Enum>), valueContainer.TypeHandlerType);
            Assert.AreEqual(typeof(System.Enum), valueContainer.Type);
           
            Assert.AreEqual("Enum : BuiltInParameter", valueContainer.TypeName);
            Assert.AreEqual("BuiltInParameter.FUNCTION_PARAM", valueContainer.ValueAsString);

            

            document.Close(false);
        }




        private Document OpenRevitFile(RevitContext revitContext)
        {
            var path = Path.Combine(revitContext.TestAssemblyLocation, @"..\..\..\assets\testmodel_rdq.rvt");
            var document = revitContext.UIApplication.Application.OpenDocumentFile(path);
            return document;
        }
    }
}
