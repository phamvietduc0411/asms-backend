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
    public class TrackingHistoryRepository : GenericRepository<TrackingHistory>, ITrackingHistoryRepository
    {
        public TrackingHistoryRepository(
            VstorageContext context,
            ILogger logger) : base(context, logger)
        {
        }

        public async Task<List<TrackingHistory>> GetWithFilterAsync(int pageNumber, int pageSize, string? orderCode)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(orderCode))
                {
                    query = query.Where(th => th.OrderCode == orderCode);
                }

                return await query
                    .OrderByDescending(th => th.CreateAt)
                    .ThenByDescending(th => th.TrackingHistoryId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tracking histories with filter.");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(string? orderCode)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(orderCode))
                {
                    query = query.Where(th => th.OrderCode == orderCode);
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
                var trackingHistory = await _dbSet.FindAsync(id);
                if (trackingHistory == null)
                {
                    return false;
                }

                _dbSet.Remove(trackingHistory);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting tracking history with id: {id}");
                throw;
            }
        }
        public async Task<IEnumerable<TrackingHistory>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TrackingHistory>> GetByOrderCodeAsync(string orderCode)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(th => th.OrderCode == orderCode)
                .OrderBy(th => th.CreateAt)
                .ToListAsync();
        }

        public async Task<TrackingHistory?> GetLatestByOrderCodeAsync(string orderCode)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(th => th.OrderCode == orderCode)
                .OrderByDescending(th => th.CreateAt)
                .ThenByDescending(th => th.TrackingHistoryId)
                .FirstOrDefaultAsync();
        }
    }
}
