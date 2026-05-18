using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SmartCity.Application;
using SmartCity.Application.Constants;
using SmartCity.Application.DTOs;
using SmartCity.Application.Settings;
using SmartCity.Domain;

namespace SmartCity.Tests
{
    [TestFixture]
    public class CommissionServiceTests
    {
        private Mock<ICacheService> _mockCacheService;
        private Mock<ICommissionRateRepository> _mockRepo;
        private Mock<IOptions<CacheSettings>> _mockOptions;
        private CommissionService _commissionService;
        private List<CommissionRate> _sampleRates;

        [SetUp]
        public void Setup()
        {
            _mockCacheService = new Mock<ICacheService>();
            _mockRepo = new Mock<ICommissionRateRepository>();
            _mockOptions = new Mock<IOptions<CacheSettings>>();

            // 1. Mock the runtime cache options config
            _mockOptions.Setup(o => o.Value).Returns(new CacheSettings
            {
                CommissionRatesExpiryMinutes = 30
            });

            // 2. Build reusable configuration rows matching production schema
            _sampleRates = new List<CommissionRate>
            {
                new CommissionRate { Id = 1, Label = "Tier 1", MinPrice = 1, MaxPrice = 10000, RatePercentage = 2.5m, IsActive = true },
                new CommissionRate { Id = 2, Label = "Tier 2", MinPrice = 10001, MaxPrice = 50000, RatePercentage = 5.0m, IsActive = true },
                new CommissionRate { Id = 3, Label = "Tier 3 (No Max)", MinPrice = 50001, MaxPrice = 0, RatePercentage = 7.5m, IsActive = true },
                new CommissionRate { Id = 4, Label = "Inactive Tier", MinPrice = 1, MaxPrice = 999999, RatePercentage = 10.0m, IsActive = false }
            };

            // 3. Initialize implementation under test
            _commissionService = new CommissionService(
                _mockCacheService.Object,
                _mockRepo.Object,
                _mockOptions.Object
            );
        }

        [Test]
        [TestCase(0)]
        [TestCase(-500.25)]
        public void CalculateAsync_WhenPriceIsZeroOrNegative_ThrowsArgumentException(decimal invalidPrice)
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commissionService.CalculateAsync(invalidPrice));

            Assert.That(ex.Message, Does.Contain("Price must be greater than zero."));
        }

        [Test]
        [TestCase(5000, 1, "Tier 1", 2.5, 125.00)]
        [TestCase(25000, 2, "Tier 2", 5.0, 1250.00)]
        public async Task CalculateAsync_WhenCacheHitsAndPriceIsBounded_ReturnsCorrectCalculation(
            decimal price, int expectedSlabId, string expectedLabel, decimal expectedPercent, decimal expectedAmount)
        {
            // Arrange: Simulate populated memory cache return pattern cleanly
            _mockCacheService
                .Setup(c => c.Get<IEnumerable<CommissionRate>>(CacheKeys.CommissionRates))
                .Returns(_sampleRates);

            // Act
            var result = await _commissionService.CalculateAsync(price);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.CommissionRateId, Is.EqualTo(expectedSlabId));
                Assert.That(result.SlabLabel, Is.EqualTo(expectedLabel));
                Assert.That(result.RatePercentage, Is.EqualTo(expectedPercent));
                Assert.That(result.CommissionAmount, Is.EqualTo(expectedAmount));
            });

            // Database repository bypass validation rule
            _mockRepo.Verify(r => r.GetAllActiveAsync(), Times.Never);
        }

        [Test]
        public async Task CalculateAsync_WhenCacheMisses_FetchesFromRepositoryAndUpdatesCache()
        {
            // Arrange: Cache returns null explicitly to trigger database retrieval fallback flow
            _mockCacheService
                .Setup(c => c.Get<IEnumerable<CommissionRate>>(CacheKeys.CommissionRates))
                .Returns((IEnumerable<CommissionRate>)null);

            _mockRepo
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(_sampleRates);

            decimal highPrice = 100000; // Tier 3 Infinite Maximum Evaluation Match

            // Act
            var result = await _commissionService.CalculateAsync(highPrice);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.CommissionRateId, Is.EqualTo(3));
                Assert.That(result.SlabLabel, Is.EqualTo("Tier 3 (No Max)"));
                Assert.That(result.CommissionAmount, Is.EqualTo(7500.00m));
            });

            // Verify infrastructure interfaces executed appropriately
            _mockRepo.Verify(r => r.GetAllActiveAsync(), Times.Once);

            // FIXED: Explicitly specified type parameter <IEnumerable<CommissionRate>> to prevent generic inference errors
            _mockCacheService.Verify(c => c.SetWithAbsoluteExpiry<IEnumerable<CommissionRate>>(
                CacheKeys.CommissionRates,
                It.IsAny<IEnumerable<CommissionRate>>(),
                TimeSpan.FromMinutes(30)), Times.Once);
        }

        [Test]
        public void CalculateAsync_WhenNoActiveSlabMatchesPrice_ThrowsInvalidOperationException()
        {
            // Arrange: Setup mock memory tracking profile to yield an empty calculation array
            _mockCacheService
                .Setup(c => c.Get<IEnumerable<CommissionRate>>(CacheKeys.CommissionRates))
                .Returns(new List<CommissionRate>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commissionService.CalculateAsync(15000));

            Assert.That(ex.Message, Does.Contain("No active commission slab for price"));
        }

        [Test]
        public async Task CalculateAsync_WhenCalculatedAmountIsFractional_RoundsToTwoDecimalPlaces()
        {
            // Arrange: Setup targeted validation schema for bankers / ceiling point rounding
            // 10,001 * 3.333% = 333.33333... -> Evaluates down to 333.33
            var preciseRates = new List<CommissionRate>
            {
                new CommissionRate { Id = 99, Label = "Precision Tier", MinPrice = 1, MaxPrice = 20000, RatePercentage = 3.333m, IsActive = true }
            };

            _mockCacheService
                .Setup(c => c.Get<IEnumerable<CommissionRate>>(CacheKeys.CommissionRates))
                .Returns(preciseRates);

            // Act
            var result = await _commissionService.CalculateAsync(10001m);

            // Assert
            Assert.That(result.CommissionAmount, Is.EqualTo(333.33m));
        }
    }
}
