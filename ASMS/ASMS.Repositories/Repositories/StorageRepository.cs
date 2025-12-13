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
                    query = query.Where(s => s.BuildingCode == buildingCode);

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
                    .AsNoTracking()
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

        public async Task<int> GetNumberOfStorageWithBuildingCode(int buildingId, string storageType)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.StorageType)
                .Where(s => s.BuildingId == buildingId
                         && s.StorageType.Name == storageType)
                .CountAsync();
        }

        public async Task<List<Storage>> GetAllStorage() => await _dbSet.Include(t => t.StorageType).ToListAsync();
        public async Task<Storage?> GetByCodeWithBuildingAsync(string storageCode)
        {
            return await _dbSet
                .Include(s => s.Building)
                .FirstOrDefaultAsync(s => s.StorageCode == storageCode);
        }

        public async Task<Storage?> GetByCodeAsNoTrackingAsync(string storageCode)
        {
            try
            {
                return await _dbSet
                    .AsNoTracking()
                    .Include(s => s.Building)
                    .Include(s => s.StorageType)
                    .FirstOrDefaultAsync(s => s.StorageCode == storageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting storage by code {Code} with no tracking", storageCode);
                throw;
            }
        }

        public async Task<Storage?> GetByCodeWithoutIncludesAsync(string storageCode)
        {
            try
            {
                return await _dbSet
                    .FirstOrDefaultAsync(s => s.StorageCode == storageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting storage by code {Code} without includes", storageCode);
                throw;
            }
        }

        public async Task<List<Storage>> GetAllAsNoTrackingAsync()
        {
            try
            {
                return await _dbSet
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all storages with no tracking");
                throw;
            }
        }
        public async Task<List<Storage>> GetAllStorageWithBuilding(bool asNoTracking = true)
        {
            var query = _dbSet
                .Include(s => s.StorageType)
                .Include(s => s.Building)
                .AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
