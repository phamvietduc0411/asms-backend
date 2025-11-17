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
        Task<IEnumerable<ContainerLocationLog>> GetAllAsync();
        Task<ContainerLocationLog?> GetByIdAsync(int id);
        Task<IEnumerable<ContainerLocationLog>> GetByContainerCodeAsync(string containerCode);
        Task<IEnumerable<ContainerLocationLog>> GetByOrderCodeAsync(string orderCode);
        Task<bool> DeleteAsync(int id);
    }
}
