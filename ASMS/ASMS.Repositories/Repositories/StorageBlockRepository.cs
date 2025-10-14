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
    public class StorageBlockRepository : GenericRepository<StorageBlock>, IStorageBlockRepository
    {
        public StorageBlockRepository(
            VstorageContext context,
            ILogger logger) : base(context, logger)
        {
        }

        public async Task<List<StorageBlock>> GetWithFilterAsync(int pageNumber, int pageSize, string? storageCode)
        {
            try
            {
                var query = _dbSet
                    .Include(sb => sb.StorageCodeNavigation)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(storageCode))
                {
                    query = query.Where(sb => sb.StorageCode == storageCode);
                }

                return await query
                    .OrderBy(sb => sb.StorageBlockCode)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting storage blocks with filter");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(string? storageCode)
        {
            try
            {
                var query = _dbSet.AsQueryable();
                if (!string.IsNullOrWhiteSpace(storageCode))
                {
                    query = query.Where(sb => sb.StorageCode == storageCode);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting total count with filter");
                throw;
            }
        }

        public async Task<StorageBlock?> GetByCodeAsync(string storageBlockCode)
        {
            try
            {
                return await _dbSet
                    .Include(sb => sb.StorageCodeNavigation)
                    .FirstOrDefaultAsync(sb => sb.StorageBlockCode == storageBlockCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting storage block with code: {Code}", storageBlockCode);
                throw;
            }
        }

        public async Task<bool> SoftDeleteAsync(string storageBlockCode)
        {
            try
            {
                var storageBlock = await _dbSet.FindAsync(storageBlockCode);
                if (storageBlock == null)
                {
                    return false;
                }

                storageBlock.IsActive = false;
                _dbSet.Update(storageBlock);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while soft deleting storage block with code: {Code}", storageBlockCode);
                throw;
            }
        }
    }
}
