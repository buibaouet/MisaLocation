using MISA.LocationV2.Web.LocationAPIControllers;
using Moq;
using Nest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.LocationV2.Web.UnitTests.LocationAPIControllers
{
    [TestFixture]
    public class LocationControllerTests
    {
        private Mock<IElasticClient> _elasticClient;
        private LocationController _locationController;

        [SetUp]
        public void Setup()
        {
            _elasticClient = new Mock<IElasticClient>();
            _locationController = new LocationController(_elasticClient.Object);
        }

        [Test]
        public void ValidateKind_KindNotANumber_EqualMinusOne()
        {
            // Arrange 
            string kindStr = "a";

            // Act
            var result = LocationController.ValidateKind(kindStr);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void ValidateKind_KindGreaterThanThree_EqualMinusOne()
        {
            // Arrange
            string kindStr = "4";

            // Act
            var result = LocationController.ValidateKind(kindStr);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void ValidateKind_ValidKind_EqualMinusOne()
        {
            // Arrange
            string kindStr = "3";

            // Act
            var result = LocationController.ValidateKind(kindStr);

            // Assert
            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task GetLocByKindAndParentID_KindLessThanZero_ErrorResponseAsync()
        {
            // Arrange
            string kind = "-1";
            string parentID = "VN01";

            // Act
            var result = await _locationController.GetLocByKindAndParentID(kind, parentID);

            // Assert
            Assert.AreEqual(1001, result.Code);
        }
    }
}
