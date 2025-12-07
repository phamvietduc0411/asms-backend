using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IFloorRepository : IGenericRepository<Floor>
    {
        Task<PaginatedList<Floor>> GetWithFilterAsync(string? shelfCode, int pageNumber, int pageSize);
        Task<Floor?> GetByCodeAsync(string floorCode);
        Task DeleteAsync(Floor entity);
        Task<List<Floor>> GetByShelfCodeAsync(string shelfCode);
        Task<List<Floor>> GetByFloorNumbersAsync(List<int> floorNumbers);
        Task<List<Floor>> GetFloorsByBuildingAndNumberAsync(int buildingId, List<int> floorNumbers);

        Task<List<Floor>> GetByShelfCodeAsNoTrackingAsync(string shelfCode);

    }
}
