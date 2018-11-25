using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HueService.Test
{
    [TestClass]
    public class UnitTest1
    {
        private readonly string _white = "FFFFFF";
        private readonly string _orange = "FFA500";
        private readonly string _red = "FF0000";

        private Mock<ILogger> _loggerMock;
        private Mock<IConfigurationSection> _configMock;
        private Mock<ILocalHueClient> _hueClientMock;

        [TestInitialize]
        public void Init()
        {
            _loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            _configMock = new Mock<IConfigurationSection>(MockBehavior.Strict);
            _hueClientMock = new Mock<ILocalHueClient>(MockBehavior.Strict);

            _configMock.Setup(m => m["UserName"]).Returns("user3");
            _hueClientMock.Setup(m => m.Initialize(_configMock.Object["UserName"]));
        }

        [TestMethod]
        public void GetTrafficColor_TravelTimeIsSameAsDefaultTravelTime_ReturnsWhite()
        {
            // Arrange
            int currentTravelTime = 40;
            int defaultTravelTime = 40;
            string expectedColor = _white;
            var hueController = new HueController(_loggerMock.Object, _configMock.Object, _hueClientMock.Object);

            //Act
            var result = hueController.GetTrafficColor(currentTravelTime, defaultTravelTime);

            //Assert
            Assert.AreEqual(expectedColor, result);
        }

        public void GetTrafficColor_TravelTimeIs4MinSloweAsDefaultTravelTime_ReturnsWhite()
        {
            // Arrange
            int currentTravelTime = 44;
            int defaultTravelTime = 40;
            string expectedColor = _white;
            var hueController = new HueController(_loggerMock.Object, _configMock.Object, _hueClientMock.Object);

            //Act
            var result = hueController.GetTrafficColor(currentTravelTime, defaultTravelTime);

            //Assert
            Assert.AreEqual(expectedColor, result);
        }

        [TestMethod]
        public void GetTrafficColor_TravelTimeIs5MinSlowerAsDefaultTravelTime_ReturnsOrange()
        {
            // Arrange
            int currentTravelTime = 45;
            int defaultTravelTime = 40;
            string expectedColor = _orange;
            var hueController = new HueController(_loggerMock.Object, _configMock.Object, _hueClientMock.Object);

            //Act
            var result = hueController.GetTrafficColor(currentTravelTime, defaultTravelTime);

            //Assert
            Assert.AreEqual(expectedColor, result);
        }

        [TestMethod]
        public void GetTrafficColor_TravelTimeIs9MinSlowerAsDefaultTravelTime_ReturnsOrange()
        {
            // Arrange
            int currentTravelTime = 49;
            int defaultTravelTime = 40;
            string expectedColor = _orange;
            var hueController = new HueController(_loggerMock.Object, _configMock.Object, _hueClientMock.Object);

            //Act
            var result = hueController.GetTrafficColor(currentTravelTime, defaultTravelTime);

            //Assert
            Assert.AreEqual(expectedColor, result);
        }

        [TestMethod]
        public void GetTrafficColor_TravelTimeIs10MinSlowerAsDefaultTravelTime_ReturnsRed()
        {
            // Arrange
            int currentTravelTime = 50;
            int defaultTravelTime = 40;
            string expectedColor = _red;
            var hueController = new HueController(_loggerMock.Object, _configMock.Object, _hueClientMock.Object);

            //Act
            var result = hueController.GetTrafficColor(currentTravelTime, defaultTravelTime);

            //Assert
            Assert.AreEqual(expectedColor, result);
        }

        [TestMethod]
        public void SetLampColorAsync_SetsTheLampColor()
        {
            // Arrange
            string lightId = "lightId221";
            string expectedColor = _red;
            IEnumerable<Light> lightList = new List<Light>() { new Light() { UniqueId = lightId } };

            _loggerMock.Setup(m => m.Information(It.Is<string>(a => a.Contains(lightId) && a.Contains(expectedColor))));
            _hueClientMock.Setup(m => m.GetLightsAsync()).Returns(Task.FromResult(lightList));

            var hueController = new HueController(_loggerMock.Object, _configMock.Object, _hueClientMock.Object);

            //Act
            var result = hueController.SetLampColorAsync(expectedColor, lightId);

            //Assert
            Assert.IsNotNull(result);
            _loggerMock.Verify(m => m.Information(It.Is<string>(a => a.Contains(lightId) && a.Contains(expectedColor))), Times.Once);
        }
    }
}
