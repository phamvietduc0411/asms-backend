using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IPricingRepository : IGenericRepository<Pricing>
    {
        Task<IEnumerable<Pricing>> GetAllAsync();
        Task<Pricing?> GetByIdAsync(int pricingId);
        Task<IEnumerable<Pricing>> GetByServiceTypeAsync(string serviceType);
        Task<Pricing?> GetByServiceAndItemAsync(string serviceType, string itemCode, bool? hasAirConditioning = null);
        Task<IEnumerable<Pricing>> GetActiveAsync();
        Task DeleteAsync(Pricing entity);
    }
}
