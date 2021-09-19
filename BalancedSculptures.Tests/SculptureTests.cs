
using System.Linq;
using ccamposh.BalancedSculptures.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CREngland.CompanyService.Tests
{
    [TestClass]
    public class SculptureTests
    {
        [TestMethod]
        public void GetChildSculptures_StringConstructor()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            Assert.AreEqual("0506070809100608091011", sculpture.ToString());
            Assert.AreEqual(11, sculpture.CurrentSize);
        }

        [TestMethod]
        public void GetChildSculptures_NoDuplicate()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var list = sculpture.GetChildSculptures();
            Assert.AreEqual(1, list.Count());
        }        

    }
}