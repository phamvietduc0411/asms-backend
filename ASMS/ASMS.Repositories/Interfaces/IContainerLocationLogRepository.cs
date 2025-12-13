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
    public interface IContainerLocationLogRepository : IGenericRepository<ContainerLocationLog>
    {
        Task<PaginatedList<ContainerLocationLog>> GetWithFilterAsync(
    string? containerCode,
    int? orderDetailId,
    int pageNumber,
    int pageSize);
        Task DeleteAsync(int id);
        Task<List<ContainerLocationLog>> GetByContainerCodeAsync(string containerCode);
        Task<ContainerLocationLog> GetLastAsync();
    }
}
