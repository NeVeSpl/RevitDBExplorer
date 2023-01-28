using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;

namespace RevitDBExplorer.Tests
{
    [TestClass]
    public class QueryParserTests
    {
        [DataTestMethod]
        [DataRow("active")]
        [DataRow("active ,")]
        [DataRow("active; ")]
        [DataRow(" , active , ")]
        public void CanParseSingleCommand(string query)
        {           
            var result = QueryParser.Parse(query).ToList();
            Assert.AreEqual("active", result[0]);
            Assert.AreEqual(1, result.Count());
        }

        [DataTestMethod]
        [DataRow("active, mark = 1")]
        [DataRow("active, mark = 1 ,")] 
        [DataRow(",active, mark = 1 ,")]
        public void CanParseTwoCommands(string query)
        {
            var result = QueryParser.Parse(query).ToList();
            Assert.AreEqual("active", result[0]);
            Assert.AreEqual("mark = 1", result[1]);
            Assert.AreEqual(2, result.Count());
        }

        [DataTestMethod]
        [DataRow("mark = 1,00,view")]
        [DataRow(",mark = 1,00,view,")]   
        public void CanParseDecimalWithComma(string query)
        {
            var result = QueryParser.Parse(query).ToList();
            Assert.AreEqual("mark = 1,00", result[0]);
            Assert.AreEqual("view", result[1]);
            Assert.AreEqual(2, result.Count());
        }

        [DataTestMethod]
        [DataRow("mark = 1,00,12345")]
        [DataRow(",mark = 1,00,12345,")]
        public void CanParseDecimalWithCommaFollowedByInteger(string query)
        {
            var result = QueryParser.Parse(query).ToList();
            Assert.AreEqual("mark = 1,00", result[0]);
            Assert.AreEqual("12345", result[1]);
            Assert.AreEqual(2, result.Count());
        }

        [DataTestMethod]
        [DataRow(",mark = 1,00m,view,")]
        public void CanParseDecimalWithCommaAndUnit(string query)
        {
            var result = QueryParser.Parse(query).ToList();
            Assert.AreEqual("mark = 1,00m", result[0]);
            Assert.AreEqual("view", result[1]);
            Assert.AreEqual(2, result.Count());
        }

        [DataTestMethod]
        [DataRow(",mark = 1,00m,12234,")]
        public void CanParseDecimalWithCommaAndUnitFollowedByInteger(string query)
        {
            var result = QueryParser.Parse(query).ToList();
            Assert.AreEqual("mark = 1,00m", result[0]);
            Assert.AreEqual("12234", result[1]);
            Assert.AreEqual(2, result.Count());
        }
    }
}