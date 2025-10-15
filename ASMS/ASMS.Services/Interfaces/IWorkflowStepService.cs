using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.WorkflowSteps;

namespace ASMS.Services.Interfaces
{
    public interface IWorkflowStepService
    {
        Task<PaginatedWorkflowStepResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? workflowTemplateName);
        Task<WorkflowStepResponse> CreateAsync(CreateWorkflowStepRequest request);
        Task<WorkflowStepResponse> UpdateAsync(int id, UpdateWorkflowStepRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
