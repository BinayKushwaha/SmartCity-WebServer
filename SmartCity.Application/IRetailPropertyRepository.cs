using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmartCity.Domain;

namespace SmartCity.Application
{
    public interface IRetailPropertyRepository
    {
        Task<RetailProperty> Create(RetailProperty property);
        Task<IEnumerable<RetailProperty>> GetPropertiesAsync();
        Task<RetailProperty> UpdateProperty(RetailProperty property);
        Task<bool> DeleteProperty(int id);
    }
}
