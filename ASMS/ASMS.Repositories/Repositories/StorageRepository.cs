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
    public class StorageRepository : GenericRepository<Storage>, IStorageRepository
    {
        public StorageRepository(VstorageContext context, ILogger logger) : base(context, logger) { }

        public async Task<List<Storage>> GetWithFilterAsync(int pageNumber, int pageSize, string? buildingCode, string? storageTypeName, string? productTypeName)
        {
            try
            {
                var query = _dbSet
                    .Include(s => s.Building)
                    .Include(s => s.StorageType)
                    .Include(s => s.ProductType)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(buildingCode))
                    query = query.Where(s => s.Building != null);

                if (!string.IsNullOrWhiteSpace(storageTypeName))
                    query = query.Where(s => s.StorageType != null && s.StorageType.Name != null && s.StorageType.Name.Contains(storageTypeName));

                if (!string.IsNullOrWhiteSpace(productTypeName))
                    query = query.Where(s => s.ProductType != null && s.ProductType.Name != null && s.ProductType.Name.Contains(productTypeName));

                return await query
                    .OrderBy(s => s.StorageCode)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting storages with filter");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(string? buildingCode, string? storageTypeName, string? productTypeName)
        {
            try
            {
                var query = _dbSet
                    .Include(s => s.Building)
                    .Include(s => s.StorageType)
                    .Include(s => s.ProductType)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(buildingCode))
                    query = query.Where(s => s.Building != null);

                if (!string.IsNullOrWhiteSpace(storageTypeName))
                    query = query.Where(s => s.StorageType != null && s.StorageType.Name != null && s.StorageType.Name.Contains(storageTypeName));

                if (!string.IsNullOrWhiteSpace(productTypeName))
                    query = query.Where(s => s.ProductType != null && s.ProductType.Name != null && s.ProductType.Name.Contains(productTypeName));

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting storages");
                throw;
            }
        }

        public async Task<Storage?> GetByCodeAsync(string storageCode)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Building)
                    .Include(s => s.StorageType)
                    .Include(s => s.ProductType)
                    .Include(s => s.Shelves)
                    .Include(s => s.OrderDetails)
                    .FirstOrDefaultAsync(s => s.StorageCode == storageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting storage by code: {Code}", storageCode);
                throw;
            }
        }

        public async Task<bool> HasRelatedDataAsync(string storageCode)
        {
            try
            {
                return await _dbSet
                    .Where(s => s.StorageCode == storageCode)
                    .AnyAsync(s => s.Shelves.Any() || s.OrderDetails.Any());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking related data for storage: {Code}", storageCode);
                throw;
            }
        }

     
    }
}
