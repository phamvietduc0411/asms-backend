using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IContainerRepository : IGenericRepository<Container>
    {
        Task<IEnumerable<Container>> GetAllAsync();
        Task<Container?> GetByCodeAsync(string code);
        Task DeleteAsync(string code);
        Task<IEnumerable<Container>> GetByFloorCodeAsync(string floorCode);
    }
}
