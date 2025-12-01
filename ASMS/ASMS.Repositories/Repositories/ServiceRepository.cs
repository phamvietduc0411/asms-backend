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
using Microsoft.IdentityModel.Tokens;

namespace ASMS.Repositories.Repositories
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {

        public ServiceRepository(
            VstorageContext context,
            ILogger logger) : base(context, logger)
        {
        }
        public async Task<List<Service>> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(nameContains))
                {
                    query = query.Where(s => s.Name != null && s.Name.Contains(nameContains));
                }

                if (minPrice.HasValue)
                {
                    query = query.Where(s => s.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query = query.Where(s => s.Price <= maxPrice.Value);
                }

                return await query
                    .OrderBy(s => s.ServiceId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting services with filter.");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(string? nameContains, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(nameContains))
                {
                    query = query.Where(s => s.Name != null && s.Name.Contains(nameContains));
                }

                if (minPrice.HasValue)
                {
                    query = query.Where(s => s.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query = query.Where(s => s.Price <= maxPrice.Value);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting total count with filter.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var service = await _dbSet.FindAsync(id);
                if (service == null)
                {
                    return false;
                }

                _dbSet.Remove(service);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting service with id: {id}");
                throw;
            }
        }
        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServiceId == id);
        }
    }
}
