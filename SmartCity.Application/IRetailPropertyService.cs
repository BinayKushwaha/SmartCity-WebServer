using SmartCity.Application.DTOs;

namespace SmartCity.Application
{
    public interface IRetailPropertyService
    {
        Task<RetailPropertyResponseDto> Create(RetailPropertyRequestDto retailPropertyDto);
        Task<RetailPropertyResponseDto> Update(RetailPropertyRequestDto retailPropertyDto);
        Task<bool> Delete(int id);
        Task<IEnumerable<RetailPropertyResponseDto>> GetRetailProperties();
    }
}
