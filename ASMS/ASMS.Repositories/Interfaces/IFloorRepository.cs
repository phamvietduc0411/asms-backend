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
        Task<IEnumerable<Floor>> GetAllAsync();
        Task<Floor?> GetByCodeAsync(string floorCode);
        Task DeleteAsync(Floor entity);
    }
}
