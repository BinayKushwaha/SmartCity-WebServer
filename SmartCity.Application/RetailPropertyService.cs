using Microsoft.AspNetCore.Http;
using SmartCity.Application.Constants;
using SmartCity.Application.DTOs;
using System.Security.Claims;

namespace SmartCity.Application
{
    public class RetailPropertyService : IRetailPropertyService
    {
        private readonly IRetailPropertyRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly ICommissionService _commissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RetailPropertyService(IRetailPropertyRepository retailPropertyRepository,
            ICacheService cacheService,
            ICommissionService commissionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = retailPropertyRepository;
            _cacheService = cacheService;
            _commissionService = commissionService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<RetailPropertyResponseDto> Create(RetailPropertyRequestDto retailPropertyDto)
        {
            var commission = await _commissionService.CalculateAsync(retailPropertyDto.Price);

            await _repository.Create(new Domain.RetailProperty()
            {
                Type = retailPropertyDto.Type,
                Features = retailPropertyDto.Features,
                Location = retailPropertyDto.Location,
                Price = retailPropertyDto.Price,
                CommissionAmount = commission.CommissionAmount,
                CommissionRateId = commission.CommissionRateId,
                BrokerId = UserId,
            });

            _cacheService.Remove(CacheKeys.AllProperties);
            await RefreshAllPropertiesCacheAsync();

            return new RetailPropertyResponseDto()
            {
                Id = retailPropertyDto.Id,
                Type = retailPropertyDto.Type,
                Features = retailPropertyDto.Features,
                Location = retailPropertyDto.Location,
                Price = retailPropertyDto.Price,
                CommissionRateId = commission.CommissionRateId,
                CommissionLabel = commission.SlabLabel,
                CommissionAmount = commission.CommissionAmount,
                CommissionRate = commission.RatePercentage
            }; ;
        }

        public async Task<bool> Delete(int id)
        {
            await _repository.DeleteProperty(id);

            _cacheService.Remove(CacheKeys.AllProperties);
            await RefreshAllPropertiesCacheAsync();

            return true;

        }

        public async Task<IEnumerable<RetailPropertyResponseDto>> GetRetailProperties()
        {
            // 1. Check cache first
            var cached = _cacheService.Get<IEnumerable<RetailPropertyResponseDto>>(CacheKeys.AllProperties);
            if (cached != null)
                return cached;

            // 2. Cache miss — fetch fresh and populate cache
            return await RefreshAllPropertiesCacheAsync();
        }

        public async Task<RetailPropertyResponseDto> Update(RetailPropertyRequestDto retailPropertyDto)
        {
            var commission = await _commissionService.CalculateAsync(retailPropertyDto.Price);

            var retailProperty = await _repository.UpdateProperty(new Domain.RetailProperty()
            {
                Id = retailPropertyDto.Id,
                Type = retailPropertyDto.Type,
                Features = retailPropertyDto.Features,
                Location = retailPropertyDto.Location,
                Price = retailPropertyDto.Price,
                CommissionAmount = commission.CommissionAmount,
                CommissionRateId = commission.CommissionRateId,
                BrokerId = UserId,
            });

            _cacheService.Remove(CacheKeys.AllProperties);
            await RefreshAllPropertiesCacheAsync();

            return new RetailPropertyResponseDto()
            {
                Id = retailPropertyDto.Id,
                Type = retailPropertyDto.Type,
                Features = retailPropertyDto.Features,
                Location = retailPropertyDto.Location,
                Price = retailPropertyDto.Price,
                CommissionRateId = commission.CommissionRateId,
                CommissionLabel = commission.SlabLabel,
                CommissionAmount = commission.CommissionAmount,
                CommissionRate = commission.RatePercentage
            };
        }

        private async Task<IEnumerable<RetailPropertyResponseDto>> RefreshAllPropertiesCacheAsync()
        {
            var freshData = await _repository.GetPropertiesAsync();


            var tasks = freshData.Select(async x =>
            {

                var commission = await _commissionService.CalculateAsync(x.Price);

                return new RetailPropertyResponseDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Features = x.Features,
                    Location = x.Location,
                    Price = x.Price,

                    CommissionRateId = commission.CommissionRateId,
                    CommissionAmount = commission.CommissionAmount,
                    CommissionLabel = commission.SlabLabel,
                    CommissionRate = commission.RatePercentage,
                    BrokerPhoneNumber= x.Broker?.PhoneNumber ?? string.Empty,
                    BrokerId = x.BrokerId,
                };
            });

            var data = (await Task.WhenAll(tasks)).ToList();

            _cacheService.SetWithSlidingExpiry(
                key: CacheKeys.AllProperties,
                value: data);

            return data;
        }

        public string UserId
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User
               .FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException("User not authenticated.");
            }
        }
    }
}
