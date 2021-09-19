using ccamposh.BalancedSculptures;
using ccamposh.BalancedSculptures.Data;
using ccamposh.BalancedSculptures.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CREngland.CompanyService.Tests
{
    [TestClass]
    public class BaseTest
    {
        private Mock<ILogger<MainService>> loggerMock;
        
        [TestInitialize]
        public void TestInitialize()
        {
            loggerMock = new Mock<ILogger<MainService>>();
        }
        protected MainService Service
        {
            get
            {
                return new MainService(
                    loggerMock.Object,
                    new SculptureRepository(),
                    new SculptureRepository()
                );
            }
        }
    }
}