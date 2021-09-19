
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CREngland.CompanyService.Tests
{
    [TestClass]
    public class MainServiceTests : BaseTest
    {
        [TestMethod]
        public void CalculateSculpturesFor6()
        {
            var result = Service.CalculateSculptures(6);
            Assert.AreEqual(18, result);
        }

        [TestMethod]
        public void CalculateSculpturesFor10()
        {
            var result = Service.CalculateSculptures(10);
            Assert.AreEqual(964, result);
        }    

        [TestMethod]
        public void CalculateSculpturesFor11()
        {
            var result = Service.CalculateSculptures(11);
            Assert.AreEqual(3036, result);
        }             
    }
}