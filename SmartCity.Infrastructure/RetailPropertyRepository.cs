using Microsoft.EntityFrameworkCore;
using SmartCity.Application;
using SmartCity.Domain;

namespace SmartCity.Infrastructure
{
    public class RetailPropertyRepository : IRetailPropertyRepository
    {
        private readonly ApplicationDbContext _context;
        public RetailPropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<RetailProperty> Create(RetailProperty property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<bool> DeleteProperty(int id)
        {
            var retailProperty = await _context.Properties.FirstOrDefaultAsync(x => x.Id == id);
            _context.Properties.Remove(retailProperty);
            await _context.SaveChangesAsync();
            return retailProperty != null;
        }

        public async Task<IEnumerable<RetailProperty>> GetPropertiesAsync()
        {
            return await _context.Properties.AsNoTracking()
                .Include(x => x.CommissionRate)
                .Include(x => x.Broker)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<RetailProperty> UpdateProperty(RetailProperty property)
        {
            var existingProperty = await _context.Properties.FirstOrDefaultAsync(x => x.Id == property.Id);
            existingProperty.Price = property.Price;
            existingProperty.Features = property.Features;
            existingProperty.Location = property.Location;
            existingProperty.Type = property.Type;
            existingProperty.CommissionRateId = property.CommissionRateId;
            existingProperty.BrokerId = property.BrokerId;
            existingProperty.UpdatedAt = DateTime.UtcNow;
            existingProperty.CommissionAmount= property.CommissionAmount;
            _context.Properties.Update(existingProperty);
            await _context.SaveChangesAsync();
            return property;
        }
    }
}
