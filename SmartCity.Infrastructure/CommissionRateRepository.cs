using Microsoft.EntityFrameworkCore;
using SmartCity.Application;
using SmartCity.Domain;

namespace SmartCity.Infrastructure
{
    public class CommissionRateRepository : ICommissionRateRepository
    {
        private readonly ApplicationDbContext _context;
        public CommissionRateRepository(ApplicationDbContext context)
        {
            _context=context;
        }
        public async Task<IEnumerable<CommissionRate>> GetAllActiveAsync()
        {
            return await _context.CommissionRates.Where(r => r.IsActive).AsNoTracking().ToListAsync();
        }
    }
}
