using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.WorkflowTemplates;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class WorkflowTemplateService : IWorkflowTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkflowTemplateService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedWorkflowTemplateResponse> GetWithFilterAsync(int pageNumber, int pageSize, int? storageTypeId, string? statusContains)
        {
            var workflowTemplates = await _unitOfWork.WorkflowTemplates.GetWithFilterAsync(pageNumber, pageSize, storageTypeId, statusContains);
            var totalCount = await _unitOfWork.WorkflowTemplates.GetTotalCountWithFilterAsync(storageTypeId, statusContains);

            var mappedTemplates = _mapper.Map<List<WorkflowTemplateResponse>>(workflowTemplates);

            return new PaginatedWorkflowTemplateResponse
            {
                Data = mappedTemplates,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<WorkflowTemplateResponse?> GetByIdAsync(int id)
        {
            var workflowTemplate = await _unitOfWork.WorkflowTemplates.GetByIdAsync(id);
            return workflowTemplate == null ? null : _mapper.Map<WorkflowTemplateResponse>(workflowTemplate);
        }

        public async Task<WorkflowTemplateResponse> CreateAsync(CreateWorkflowTemplateRequest request)
        {
            var existing = await _unitOfWork.WorkflowTemplates.GetByIdAsync(request.WorkflowTemplateId);
            if (existing != null)
            {
                throw new Exception($"Workflow template with id {request.WorkflowTemplateId} already exists.");
            }

            var workflowTemplate = _mapper.Map<WorkflowTemplate>(request);
            var created = await _unitOfWork.WorkflowTemplates.AddAsync(workflowTemplate);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<WorkflowTemplateResponse>(created);
        }

        public async Task<WorkflowTemplateResponse> UpdateAsync(int id, UpdateWorkflowTemplateRequest request)
        {
            var existing = await _unitOfWork.WorkflowTemplates.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception($"Workflow template with id {id} not found.");
            }

            _mapper.Map(request, existing);
            await _unitOfWork.WorkflowTemplates.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<WorkflowTemplateResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.WorkflowTemplates.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
            }
            return result;
        }
    }
}
