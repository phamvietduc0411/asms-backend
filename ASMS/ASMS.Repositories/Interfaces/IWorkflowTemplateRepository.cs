using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IWorkflowTemplateRepository : IGenericRepository<WorkflowTemplate>
    {
        Task<List<WorkflowTemplate>> GetWithFilterAsync(int pageNumber, int pageSize, int? storageTypeId, string? statusContains);
        Task<int> GetTotalCountWithFilterAsync(int? storageTypeId, string? statusContains);
        Task<WorkflowTemplate?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
