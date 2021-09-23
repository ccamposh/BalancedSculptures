
using ccamposh.BalancedSculptures.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CREngland.CompanyService.Tests
{
    [TestClass]
    public class MainServiceTests : BaseTest
    {
        [TestMethod]
        public void CalculateSculpturesFor6()
        {
            var result = Service.CalculateSculptures(6, Sculpture.BaseSculpture);
            Assert.AreEqual(18, result);
        }

        [TestMethod]
        public void CalculateSculpturesFor10()
        {
            var result = Service.CalculateSculptures(10, Sculpture.BaseSculpture);
            Assert.AreEqual(964, result);
        }    

        [TestMethod]
        public void CalculateSculpturesFor11()
        {
            var result = Service.CalculateSculptures(11, Sculpture.BaseSculpture);
            Assert.AreEqual(3036, result);
        }    

        [TestMethod]
        public void CalculateSculpturesFor18_FinalSteps()
        {
            var result = Service.CalculateSculptures(18, Sculpture.StringToArray("08080808080808080808080808080808"));
            //result should be two on top + 16+15+14+13+...til 1 = 137
            Assert.AreEqual(137, result);
        }   

        [TestMethod]
        public void CalculateSculpturesFor18_FinalSteps2()
        {
            var result = Service.CalculateSculptures(18, Sculpture.StringToArray("080808080808080808080808080808"));
            //result should be two on top + 16+15+14+13+...til 1 = 137
            Assert.AreEqual(137, result);
        }                          
    }
}