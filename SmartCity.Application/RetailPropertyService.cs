using SmartCity.Application.DTOs;

namespace SmartCity.Application
{
    public class RetailPropertyService : IRetailPropertyService
    {
        private readonly IRetailPropertyRepository _repository;
        public RetailPropertyService(IRetailPropertyRepository retailPropertyRepository)
        {
            _repository = retailPropertyRepository;
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
            return retailPropertyDto;
        }

        public async Task<bool> Delete(int id)
        {
            await _repository.DeleteProperty(id);
            return true;

        }

        public async Task<IEnumerable<RetailPropertyDto>> GetRetailProperties()
        {
            var properties = await _repository.GetProperties();
            return properties.Select(x => new RetailPropertyDto()
            {
                Id = x.Id,
                Type = x.Type,
                Features = x.Features,
                Location = x.Location,
                Price = x.Price,
            }).ToList();
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
            return retailPropertyDto;
        }
    }
}
