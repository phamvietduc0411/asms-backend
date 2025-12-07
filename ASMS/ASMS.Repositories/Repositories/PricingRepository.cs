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
    public class PricingRepository : GenericRepository<Pricing>, IPricingRepository
    {
        public PricingRepository(VstorageContext context, ILogger logger) : base(context, logger) { }

        public async Task<IEnumerable<Pricing>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<Pricing?> GetByIdAsync(int pricingId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PricingId == pricingId);
        }

        public async Task<IEnumerable<Pricing>> GetByServiceTypeAsync(string serviceType)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.ServiceType == serviceType && p.IsActive == true)
                .ToListAsync();
        }

        public async Task<Pricing?> GetByServiceAndItemAsync(string serviceType, string itemCode, bool? hasAirConditioning = null)
        {
            var query = _dbSet.AsNoTracking()
                .Where(p => p.ServiceType == serviceType && p.ItemCode == itemCode && p.IsActive == true);

            if (hasAirConditioning.HasValue)
            {
                query = query.Where(p => p.HasAirConditioning == hasAirConditioning.Value);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Pricing>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.IsActive == true)
                .ToListAsync();
        }

        public async Task DeleteAsync(Pricing entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }
    }
}
