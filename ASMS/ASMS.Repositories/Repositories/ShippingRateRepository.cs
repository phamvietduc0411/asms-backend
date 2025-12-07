using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class ShippingRateRepository : GenericRepository<ShippingRate>, IShippingRateRepository
    {
        public ShippingRateRepository(VstorageContext context, ILogger logger) : base(context, logger) { }

        public async Task<IEnumerable<ShippingRate>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<ShippingRate?> GetByIdAsync(int shippingRateId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.ShippingRateId == shippingRateId);
        }

        public async Task<ShippingRate?> FindRateAsync(decimal distanceKm, int containerQuantity)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(s => s.IsActive == true)
                .Where(s => s.DistanceMinKm <= distanceKm &&
                           (s.DistanceMaxKm == null || distanceKm <= s.DistanceMaxKm))
                .Where(s => s.ContainerQtyMin <= containerQuantity &&
                           (s.ContainerQtyMax == null || containerQuantity <= s.ContainerQtyMax))
                .OrderByDescending(s => s.DistanceMinKm)
                .ThenByDescending(s => s.ContainerQtyMin)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShippingRate>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(s => s.IsActive == true)
                .ToListAsync();
        }

        public async Task DeleteAsync(ShippingRate entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }
    }
}
