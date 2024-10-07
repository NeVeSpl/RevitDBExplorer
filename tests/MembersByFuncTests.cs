using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure.StructuralSections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitTestLibrary;

namespace RevitDBExplorer.Tests
{
    [TestClass]
    public class MembersByFuncTests
    {
        [RevitTestMethod]
        public void Static_ForDocument_AsArgument_1_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Document>.ByFunc((doc, target) => Document.GetDocumentVersion(doc));
            var accessor =  memberOverride.MemberAccessorFactory();

            Assert.AreEqual("Document.GetDocumentVersion(document)", accessor.DefaultInvocation.Syntax);
        }       
     
        [RevitTestMethod]
        public void Static_ForDocument_AsArgument_2_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Document>.ByFunc((doc, target) => Document.GetDocumentVersion(target));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("Document.GetDocumentVersion(document)", accessor.DefaultInvocation.Syntax);
        }

        [RevitTestMethod]
        public void Static_ForDocument_AsArgument_3_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Document>.ByFunc((doc, target) => BasicFileInfo.Extract(target.PathName));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("BasicFileInfo.Extract(document.PathName)", accessor.DefaultInvocation.Syntax);
        }

        [RevitTestMethod]
        public void Static_ForFamilyInstance_AsArgument_1_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<FamilyInstance>.ByFunc((doc, target) => StructuralSectionUtils.GetStructuralSection(doc, target.Id));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("StructuralSectionUtils.GetStructuralSection(document, item.Id)", accessor.DefaultInvocation.Syntax);
        }


        [RevitTestMethod]
        public void Instance_ForDocument_AsVar_1_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Document>.ByFunc((doc, target) => doc.GetChangedElements(Guid.Empty));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("document.GetChangedElements(Guid.Empty)", accessor.DefaultInvocation.Syntax);
        }

        [RevitTestMethod]
        public void Instance_ForDocument_AsVar_2_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Document>.ByFunc((doc, target) => target.GetChangedElements(Guid.Empty));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("document.GetChangedElements(Guid.Empty)", accessor.DefaultInvocation.Syntax);
        }


        [RevitTestMethod]
        public void Instance_ForElement_AsArgument_1_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Element>.ByFunc((doc, target) => doc.ActiveView.GetElementOverrides(target.Id));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("document.ActiveView.GetElementOverrides(item.Id)", accessor.DefaultInvocation.Syntax);
        }

        [RevitTestMethod]
        public void Instance_ForElement_AsArgument_2_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Element>.ByFunc((doc, target) => doc.GetWorksetId(target.Id));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("document.GetWorksetId(item.Id)", accessor.DefaultInvocation.Syntax);
        }

        [RevitTestMethod]
        public void Instance_ForReference_AsArgument_1_Test(RevitContext revitContext)
        {
            var memberOverride = MemberOverride<Reference>.ByFunc((doc, target) => doc.GetElement(target.ElementId).GetGeometryObjectFromReference(target));
            var accessor = memberOverride.MemberAccessorFactory();

            Assert.AreEqual("document.GetElement(item.ElementId).GetGeometryObjectFromReference(item)", accessor.DefaultInvocation.Syntax);
        }


    }
}
