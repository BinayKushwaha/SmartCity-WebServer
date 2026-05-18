using Microsoft.AspNetCore.Http;
using Moq;
using SmartCity.Application;
using SmartCity.Application.Constants;
using SmartCity.Application.DTOs;
using SmartCity.Domain;
using System.Security.Claims;

namespace SmartCity.NTest
{
    [TestFixture]
    public class RetailPropertyServiceTests
    {
        private Mock<IRetailPropertyRepository> _mockRepo;
        private Mock<ICacheService> _mockCache;
        private Mock<ICommissionService> _mockCommissionService;
        private Mock<IHttpContextAccessor> _mockHttpAccessor;
        private RetailPropertyService _service;

        private const string TestBrokerId = "broker-abc-123";

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRetailPropertyRepository>();
            _mockCache = new Mock<ICacheService>();
            _mockCommissionService = new Mock<ICommissionService>();
            _mockHttpAccessor = new Mock<IHttpContextAccessor>();

            // Mock the user authentication context framework 
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, TestBrokerId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            _mockHttpAccessor.Setup(h => h.HttpContext).Returns(httpContext);

            // Construct the implementation package under test
            _service = new RetailPropertyService(
                _mockRepo.Object,
                _mockCache.Object,
                _mockCommissionService.Object,
                _mockHttpAccessor.Object
            );
        }

        #region Context / Auth Tests

        [Test]
        public void UserId_WhenUserIsNotAuthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange: Sever the HTTP runtime context state
            _mockHttpAccessor.Setup(h => h.HttpContext).Returns((HttpContext)null);

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => _ = _service.UserId);
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ValidPayload_CalculatesCommissionSavesToRepoAndRefreshesCache()
        {
            // Arrange
            var requestDto = new RetailPropertyRequestDto
            {
                Id = 10,
                Type = PropertyType.Agricultural,
                Features = "High Foot Traffic",
                Location = "Downtown Central",
                Price = 500000m
            };

            var commissionDto = new CommissionResultDto
            {
                CommissionRateId = 3,
                SlabLabel = "Premium Tier",
                RatePercentage = 5.5m,
                CommissionAmount = 27500m
            };

            _mockCommissionService
                .Setup(c => c.CalculateAsync(requestDto.Price))
                .ReturnsAsync(commissionDto);

            _mockRepo
                .Setup(r => r.GetPropertiesAsync())
                .ReturnsAsync(new List<RetailProperty>()); // Empty lookup array fallback for cache renewal

            // Act
            var result = await _service.Create(requestDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(requestDto.Id));
                Assert.That(result.CommissionAmount, Is.EqualTo(commissionDto.CommissionAmount));
                Assert.That(result.CommissionLabel, Is.EqualTo(commissionDto.SlabLabel));
                Assert.That(result.CommissionRate, Is.EqualTo(commissionDto.RatePercentage));
            });

            // Verify infrastructure assertions executed correctly
            _mockCommissionService.Verify(c => c.CalculateAsync(requestDto.Price), Times.Exactly(1));
            _mockRepo.Verify(r => r.Create(It.Is<RetailProperty>(p =>
                p.Price == requestDto.Price &&
                p.BrokerId == TestBrokerId &&
                p.CommissionAmount == commissionDto.CommissionAmount
            )), Times.Once);

            _mockCache.Verify(c => c.Remove(CacheKeys.AllProperties), Times.Once);
            _mockCache.Verify(c => c.SetWithSlidingExpiry(CacheKeys.AllProperties, It.IsAny<object>()), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ValidId_InvokesRepositoryRemovesAndRefreshesCache()
        {
            // Arrange
            int targetId = 44;
            _mockRepo.Setup(r => r.GetPropertiesAsync()).ReturnsAsync(new List<RetailProperty>());

            // Act
            var result = await _service.Delete(targetId);

            // Assert
            Assert.That(result, Is.True);
            _mockRepo.Verify(r => r.DeleteProperty(targetId), Times.Once);
            _mockCache.Verify(c => c.Remove(CacheKeys.AllProperties), Times.Once);
            _mockCache.Verify(c => c.SetWithSlidingExpiry(CacheKeys.AllProperties, It.IsAny<object>()), Times.Once);
        }

        #endregion

        #region Get / Cache Strategy Tests

        [Test]
        public async Task GetRetailProperties_WhenCacheHits_ReturnsCachedCollectionsDirectlyWithoutDatabaseTouch()
        {
            // Arrange
            var cachedPayload = new List<RetailPropertyResponseDto>
            {
                new RetailPropertyResponseDto { Id = 1, Location = "Cached Street A" },
                new RetailPropertyResponseDto { Id = 2, Location = "Cached Street B" }
            };

            _mockCache
                .Setup(c => c.Get<IEnumerable<RetailPropertyResponseDto>>(CacheKeys.AllProperties))
                .Returns(cachedPayload);

            // Act
            var result = await _service.GetRetailProperties();

            // Assert
            Assert.That(result, Is.EquivalentTo(cachedPayload));
            _mockRepo.Verify(r => r.GetPropertiesAsync(), Times.Never);
        }

        [Test]
        public async Task GetRetailProperties_WhenCacheMisses_FetchesFromDatabaseCalculatesCommissionsAndSetsCache()
        {
            // Arrange
            _mockCache
                .Setup(c => c.Get<IEnumerable<RetailPropertyResponseDto>>(CacheKeys.AllProperties))
                .Returns((IEnumerable<RetailPropertyResponseDto>)null);

            var databaseEntities = new List<RetailProperty>
            {
                new RetailProperty { Id = 1, Price = 100000m, Location = "Main Plaza" }
            };

            var calculatedCommission = new CommissionResultDto
            {
                CommissionRateId = 1,
                CommissionAmount = 2000m,
                SlabLabel = "Standard",
                RatePercentage = 2.0m
            };

            _mockRepo.Setup(r => r.GetPropertiesAsync()).ReturnsAsync(databaseEntities);
            _mockCommissionService.Setup(c => c.CalculateAsync(100000m)).ReturnsAsync(calculatedCommission);

            // Act
            var result = (await _service.GetRetailProperties()).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].Id, Is.EqualTo(1));
                Assert.That(result[0].CommissionAmount, Is.EqualTo(2000m));
                Assert.That(result[0].Location, Is.EqualTo("Main Plaza"));
            });

            _mockRepo.Verify(r => r.GetPropertiesAsync(), Times.Once);
            _mockCache.Verify(c => c.SetWithSlidingExpiry(CacheKeys.AllProperties, It.IsAny<List<RetailPropertyResponseDto>>()), Times.Once);
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ValidPayload_UpdatesDataAndCalculatesNewCommissionMatrix()
        {
            // Arrange
            var updateRequestDto = new RetailPropertyRequestDto
            {
                Id = 5,
                Type = PropertyType.Residential,
                Price = 800000m,
                Location = "Industrial Zone"
            };

            var commissionUpdateDto = new CommissionResultDto
            {
                CommissionRateId = 5,
                SlabLabel = "Bulk Slab",
                RatePercentage = 1.5m,
                CommissionAmount = 12000m
            };

            _mockCommissionService
                .Setup(c => c.CalculateAsync(updateRequestDto.Price))
                .ReturnsAsync(commissionUpdateDto);

            _mockRepo.Setup(r => r.GetPropertiesAsync()).ReturnsAsync(new List<RetailProperty>());

            // Act
            var result = await _service.Update(updateRequestDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(updateRequestDto.Id));
                Assert.That(result.CommissionAmount, Is.EqualTo(12000m));
                Assert.That(result.CommissionLabel, Is.EqualTo("Bulk Slab"));
            });

            _mockRepo.Verify(r => r.UpdateProperty(It.Is<RetailProperty>(p =>
                p.Id == updateRequestDto.Id &&
                p.Price == updateRequestDto.Price &&
                p.BrokerId == TestBrokerId
            )), Times.Once);

            _mockCache.Verify(c => c.Remove(CacheKeys.AllProperties), Times.Once);
        }

        #endregion
    }
}
