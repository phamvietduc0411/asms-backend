using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface ITrackingHistoryRepository : IGenericRepository<TrackingHistory>
    {
        Task<List<TrackingHistory>> GetWithFilterAsync(int pageNumber, int pageSize, string? orderCode, string? currentAssign, string? nextAssign);
        Task<int> GetTotalCountWithFilterAsync(string? orderCode, string? currentAssign, string? nextAssign);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<TrackingHistory>> GetByOrderCodeAsync(string orderCode);
        Task<TrackingHistory?> GetLatestByOrderCodeAsync(string orderCode);
        Task<IEnumerable<TrackingHistory>> GetAllAsync();
    }
}
