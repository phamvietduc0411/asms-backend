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
    public class StorageTypeRepository : GenericRepository<StorageType>, IStorageTypeRepository
    {
        public StorageTypeRepository(VstorageContext context, ILogger logger) : base(context, logger) { }

        public async Task<List<StorageType>> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(nameContains))
                    query = query.Where(st => st.Name != null && st.Name.Contains(nameContains));

                return await query
                    .OrderBy(st => st.StorageTypeId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting storage types with filter");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(string? nameContains)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(nameContains))
                    query = query.Where(st => st.Name != null && st.Name.Contains(nameContains));

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting storage types");
                throw;
            }
        }

        public async Task<StorageType?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting storage type by id: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var storageType = await _dbSet.FindAsync(id);
                if (storageType == null) return false;

                _dbSet.Remove(storageType);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting storage type with id: {Id}", id);
                throw;
            }
        }
    }
}
