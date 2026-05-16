using SmartCity.Application.DTOs;

namespace SmartCity.Application
{
    public interface IRetailPropertyService
    {
        Task<RetailPropertyDto> Create(RetailPropertyDto retailPropertyDto);
        Task<RetailPropertyDto> Update(RetailPropertyDto retailPropertyDto);
        Task<bool> Delete(int id);
        Task<IEnumerable<RetailPropertyDto>> GetRetailProperties();
    }
}
