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
    public interface IBuildingRepository : IGenericRepository<Building>
    {
        Task<Building> GetLastRecord();
        Task<Building?> GetByCodeAsync(string buildingCode);
        Task<PaginatedList<Building>> GetAllAsync(int pageNumber, int pageSize);
    }
}
