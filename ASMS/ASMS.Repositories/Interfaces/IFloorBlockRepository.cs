using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IFloorBlockRepository : IGenericRepository<FloorBlock>
    {
        Task<IEnumerable<FloorBlock>> GetAllAsync();
        Task<FloorBlock?> GetByCodeAsync(string floorBlockCode);
        Task DeleteAsync(string floorBlockCode);
    }
}
