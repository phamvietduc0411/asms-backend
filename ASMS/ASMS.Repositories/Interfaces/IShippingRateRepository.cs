using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IShippingRateRepository : IGenericRepository<ShippingRate>
    {
        Task<IEnumerable<ShippingRate>> GetAllAsync();
        Task<ShippingRate?> GetByIdAsync(int shippingRateId);
        Task<ShippingRate?> FindRateAsync(decimal distanceKm, int containerQuantity);
        Task<IEnumerable<ShippingRate>> GetActiveAsync();
        Task DeleteAsync(ShippingRate entity);
    }
}
