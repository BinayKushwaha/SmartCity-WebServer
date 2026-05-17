using SmartCity.Domain;

namespace SmartCity.Application
{
    public interface ICommissionRateRepository
    {
        Task<IEnumerable<CommissionRate>> GetAllActiveAsync();
    }
}
