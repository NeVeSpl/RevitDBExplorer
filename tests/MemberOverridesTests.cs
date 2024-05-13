using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitTestLibrary;

namespace RevitDBExplorer.Tests
{
    [TestClass]
    public class MemberOverridesTests
    {
        [RevitTestMethod]
        public void AssetProperties_Item_Test(RevitContext revitContext)
        {
            var document = OpenRevitFile(revitContext);
            var context = new SnoopableContext() { Document = document };
            var asset = revitContext.UIApplication.Application.GetAssets(Autodesk.Revit.DB.Visual.AssetType.Content).FirstOrDefault();
            var memberUnderTest = new AssetProperties_Item() as IAccessorForDefaultPresenter;

            var readResult = memberUnderTest.Read(context, asset);
            Assert.AreEqual("[AssetProperty: 24]", readResult.Label);
            Assert.AreEqual(true, readResult.CanBeSnooped);

            var snoopResult = memberUnderTest.Snoop(context, asset, null).ToArray();
            Assert.AreEqual(asset.Size, snoopResult.Length);

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
