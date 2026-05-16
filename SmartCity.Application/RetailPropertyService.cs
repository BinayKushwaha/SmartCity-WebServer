using SmartCity.Application.Constants;
using SmartCity.Application.DTOs;

namespace SmartCity.Application
{
    public class RetailPropertyService : IRetailPropertyService
    {
        private readonly IRetailPropertyRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public RetailPropertyService(IRetailPropertyRepository retailPropertyRepository,
            ICacheService cacheService)
        {
            _repository = retailPropertyRepository;
            _cacheService = cacheService;
        }
        public async Task<RetailPropertyDto> Create(RetailPropertyDto retailPropertyDto)
        {
            await _repository.Create(new Domain.RetailProperty()
            {
                Type = retailPropertyDto.Type,
                Features = retailPropertyDto.Features,
                Location = retailPropertyDto.Location,
                Price = retailPropertyDto.Price,
            });

            await RefreshAllPropertiesCacheAsync();

            return retailPropertyDto;
        }

        public async Task<bool> Delete(int id)
        {
            await _repository.DeleteProperty(id);

            _cacheService.Remove(CacheKeys.GetPropertyById(id));

            await RefreshAllPropertiesCacheAsync();

            return true;

        }

        public async Task<IEnumerable<RetailPropertyDto>> GetRetailProperties()
        {
            // 1. Check cache first
            var cached = _cacheService.Get<IEnumerable<RetailPropertyDto>>(CacheKeys.AllProperties);
            if (cached != null)
                return cached;  // ✅ cache hit — no DB call

            // 2. Cache miss — fetch fresh and populate cache
            return await RefreshAllPropertiesCacheAsync();
        }

        public async Task<RetailPropertyDto> Update(RetailPropertyDto retailPropertyDto)
        {
            var retailProperty = await _repository.UpdateProperty(new Domain.RetailProperty()
            {
                Id = retailPropertyDto.Id,
                Type = retailPropertyDto.Type,
                Features = retailPropertyDto.Features,
                Location = retailPropertyDto.Location,
                Price = retailPropertyDto.Price,
            });

            await RefreshAllPropertiesCacheAsync();

            return retailPropertyDto;
        }

        private async Task<IEnumerable<RetailPropertyDto>> RefreshAllPropertiesCacheAsync()
        {
            // 1. Fetch latest data from DB
            var freshData = await _repository.GetPropertiesAsync();
            var Data = freshData.Select(x => new RetailPropertyDto()
            {
                Id = x.Id,
                Type = x.Type,
                Features = x.Features,
                Location = x.Location,
                Price = x.Price,
            }).ToList();

            // 2. Overwrite cache with fresh data
            _cacheService.Set(
                key: CacheKeys.AllProperties,
                value: Data,
                expiration: _cacheDuration);

            return Data;
        }
    }
}
