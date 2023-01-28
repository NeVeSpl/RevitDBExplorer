using System.Linq;
using Autodesk.Revit.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitTestLibrary.MSTest;

namespace RevitDBExplorer.Tests.RDQ
{
    [TestClass]
    public class CommandParserTests
    {
        [RevitTestMethod]
        //[DataRow("123456")]
        // [DataRow("i:123456")]
        public void CanParseElementIdCommand(UIApplication uia)
        {
            var result = CommandParser.Parse("123456").ToList();
            Assert.AreEqual(Domain.RevitDatabaseQuery.CmdType.ElementId, result[0].Type);
           
        }
    }
}
