using ASMS.Services.Model.ContainerLocationLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IContainerLocationLogService
    {
        Task<IEnumerable<ContainerLocationLogResponse>> GetAllAsync();
        Task<ContainerLocationLogResponse?> GetByIdAsync(int id);
        Task<ContainerLocationLogResponse> CreateAsync(CreateContainerLocationLogRequest request);
        Task<ContainerLocationLogResponse?> UpdateAsync(int id, UpdateContainerLocationLogRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
