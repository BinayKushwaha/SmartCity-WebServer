using SmartCity.Application.DTOs;

namespace SmartCity.Application
{
    public interface ICommissionService
    {
        Task<CommissionResultDto> CalculateAsync(decimal price);
    }
}
