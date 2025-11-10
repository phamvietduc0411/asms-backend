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
    public interface IContainerRepository : IGenericRepository<Container>
    {
        Task<PaginatedList<Container>> GetAllAsync(int pageNumber, int pageSize);
        Task<Container?> GetByCodeAsync(string containerCode);
        Task DeleteAsync(string code);
        Task<List<Container>> GetByFloorCodeAsync(string floorCode);
        Task<List<Container>> GetAvailableByTypeAsync(int containerTypeId);
        Task UpdateAsync(Container container);

    }
}
