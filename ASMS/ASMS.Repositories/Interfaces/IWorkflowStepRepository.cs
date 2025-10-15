using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IWorkflowStepRepository : IGenericRepository<WorkflowStep>
    {
        Task<List<WorkflowStep>> GetWithFilterAsync(int pageNumber, int pageSize, string? workflowTemplateName);
        Task<int> GetTotalCountWithFilterAsync(string? workflowTemplateName);
        Task<bool> DeleteAsync(int id);
    }
}
