using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.WorkflowTemplates;

namespace ASMS.Services.Interfaces
{
    public interface IWorkflowTemplateService
    {
        Task<PaginatedWorkflowTemplateResponse> GetWithFilterAsync(int pageNumber, int pageSize, int? storageTypeId, string? statusContains);
        Task<WorkflowTemplateResponse?> GetByIdAsync(int id);
        Task<WorkflowTemplateResponse> CreateAsync(CreateWorkflowTemplateRequest request);
        Task<WorkflowTemplateResponse> UpdateAsync(int id, UpdateWorkflowTemplateRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
