using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartCity.Application;
using SmartCity.Application.DTOs;
using SmartCity_WebServer.Controllers;

namespace SmartCity.NTest
{
    [TestFixture]
    public class RetailPropertyControllerTests
    {
        private Mock<IRetailPropertyService> _mockService;
        private RetailPropertyController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IRetailPropertyService>();
            _controller = new RetailPropertyController(_mockService.Object);
        }

        #region Create Method Tests

        [Test]
        public async Task Create_ValidModel_ReturnsOkWithResult()
        {
            // Arrange
            var requestDto = new RetailPropertyRequestDto { Price = 250000m, Location = "City Center" };
            var responseDto = new RetailPropertyResponseDto { Id = 1, Price = 250000m, Location = "City Center" };

            _mockService.Setup(s => s.Create(requestDto)).ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Create(requestDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(responseDto));
        }

        [Test]
        public async Task Create_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var requestDto = new RetailPropertyRequestDto();
            _controller.ModelState.AddModelError("Price", "Price is required.");

            // Act
            var result = await _controller.Create(requestDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            _mockService.Verify(s => s.Create(It.IsAny<RetailPropertyRequestDto>()), Times.Never);
        }

        #endregion

        #region Update Method Tests

        [Test]
        public async Task Update_ValidRequest_ReturnsOkWithUpdatedResult()
        {
            // Arrange
            int propertyId = 5;
            var requestDto = new RetailPropertyRequestDto { Price = 300000m };
            var responseDto = new RetailPropertyResponseDto { Id = propertyId, Price = 300000m };

            _mockService.Setup(s => s.Update(It.Is<RetailPropertyRequestDto>(r => r.Id == propertyId)))
                        .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Update(propertyId, requestDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(responseDto));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Update_InvalidId_ReturnsBadRequest(int invalidId)
        {
            // Arrange
            var requestDto = new RetailPropertyRequestDto { Price = 300000m };

            // Act
            var result = await _controller.Update(invalidId, requestDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid property ID."));
        }

        [Test]
        public async Task Update_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            int propertyId = 5;
            var requestDto = new RetailPropertyRequestDto();
            _controller.ModelState.AddModelError("Location", "Location is required.");

            // Act
            var result = await _controller.Update(propertyId, requestDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Update_PropertyNotFoundInService_ReturnsNotFound()
        {
            // Arrange
            int propertyId = 99;
            var requestDto = new RetailPropertyRequestDto { Price = 400000m };

            _mockService.Setup(s => s.Update(It.IsAny<RetailPropertyRequestDto>()))
                        .ReturnsAsync((RetailPropertyResponseDto)null);

            // Act
            var result = await _controller.Update(propertyId, requestDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo($"Property with ID {propertyId} not found."));
        }

        #endregion

        #region GetProperties Method Tests

        [Test]
        public async Task GetProperties_PropertiesExist_ReturnsOkWithList()
        {
            // Arrange
            var mockProperties = new List<RetailPropertyResponseDto>
            {
                new RetailPropertyResponseDto { Id = 1, Location = "Location A" },
                new RetailPropertyResponseDto { Id = 2, Location = "Location B" }
            };

            _mockService.Setup(s => s.GetRetailProperties()).ReturnsAsync(mockProperties);

            // Act
            var result = await _controller.GetProperties();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EquivalentTo(mockProperties));
        }

        [Test]
        public async Task GetProperties_NoPropertiesFound_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetRetailProperties()).ReturnsAsync(new List<RetailPropertyResponseDto>());

            // Act
            var result = await _controller.GetProperties();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
            Assert.That(notFoundResult.Value, Is.EqualTo("No properties found."));
        }

        [Test]
        public async Task GetProperties_ServiceThrowsException_ReturnsInternalServerError500()
        {
            // Arrange
            _mockService.Setup(s => s.GetRetailProperties()).ThrowsAsync(new Exception("Database connection failure"));

            // Act
            var result = await _controller.GetProperties();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
            Assert.That(statusCodeResult.Value, Is.EqualTo("Internal error occured."));
        }

        #endregion

        #region Delete Method Tests

        [Test]
        public async Task Delete_ValidId_CallsServiceAndReturnsOk()
        {
            // Arrange
            int targetId = 12;
            _mockService.Setup(s => s.Delete(targetId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(targetId);

            // Assert
            var okResult = result as OkResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            _mockService.Verify(s => s.Delete(targetId), Times.Once);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-5)]
        public async Task Delete_InvalidId_ReturnsBadRequestWithoutCallingService(int invalidId)
        {
            // Act
            var result = await _controller.Delete(invalidId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid property ID."));

            _mockService.Verify(s => s.Delete(It.IsAny<int>()), Times.Never);
        }

        #endregion
    }
}
