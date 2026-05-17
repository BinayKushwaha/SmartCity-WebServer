using Microsoft.Extensions.Options;
using SmartCity.Application.Constants;
using SmartCity.Application.DTOs;
using SmartCity.Application.Settings;
using SmartCity.Domain;

namespace SmartCity.Application
{
    public class CommissionService : ICommissionService
    {
        private readonly ICacheService _cacheService;
        private readonly ICommissionRateRepository _commissionRateRepository;
        private readonly CacheSettings _cacheSettings;
        public CommissionService(ICacheService cacheService,
            ICommissionRateRepository commissionRateRepository,
            IOptions<CacheSettings> cacheSettings)
        {
            _cacheService = cacheService;
            _commissionRateRepository = commissionRateRepository;
            _cacheSettings = cacheSettings.Value;
        }
        public async Task<CommissionResultDto> CalculateAsync(decimal price)
        {
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            // Match price to correct slab
            var rates = await GetCachedRatesAsync();
            var slab = rates
                .Where(r => r.IsActive)
                .Where(r => price >= r.MinPrice &&
                           (r.MaxPrice == 0 || price <= r.MaxPrice))
                .OrderBy(r => r.MinPrice)
                .FirstOrDefault()
                    ?? throw new InvalidOperationException(
                           $"No active commission slab for price {price:N0}.");

            var amount = Math.Round(price * slab.RatePercentage / 100, 2);

            return new CommissionResultDto
            {
                CommissionRateId = slab.Id,
                SlabLabel = slab.Label,
                RatePercentage = slab.RatePercentage,
                CommissionAmount = amount
            };
        }
        private async Task<IEnumerable<CommissionRate>> GetCachedRatesAsync()
        {
            var cached = _cacheService
                .Get<IEnumerable<CommissionRate>>(CacheKeys.CommissionRates);
            return cached ?? await RefreshRatesCacheAsync();
        }
        private async Task<IEnumerable<CommissionRate>> RefreshRatesCacheAsync()
        {
            var fresh = await _commissionRateRepository.GetAllActiveAsync();
            _cacheService.SetWithAbsoluteExpiry(
            key: CacheKeys.CommissionRates,
            value: fresh,
            expiration: TimeSpan.FromMinutes(
                _cacheSettings.CommissionRatesExpiryMinutes));
            return fresh;
        }
    }
}
