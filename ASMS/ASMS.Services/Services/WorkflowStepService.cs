using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.WorkflowSteps;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class WorkflowStepService : IWorkflowStepService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkflowStepService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedWorkflowStepResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? workflowTemplateName)
        {
            var workflowSteps = await _unitOfWork.WorkflowSteps.GetWithFilterAsync(pageNumber, pageSize, workflowTemplateName);
            var totalCount = await _unitOfWork.WorkflowSteps.GetTotalCountWithFilterAsync(workflowTemplateName);

            var mappedSteps = _mapper.Map<List<WorkflowStepResponse>>(workflowSteps);

            return new PaginatedWorkflowStepResponse
            {
                Data = mappedSteps,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<WorkflowStepResponse> CreateAsync(CreateWorkflowStepRequest request)
        {
            var workflowStep = _mapper.Map<WorkflowStep>(request);
            var created = await _unitOfWork.WorkflowSteps.AddAsync(workflowStep);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<WorkflowStepResponse>(created);
        }

        public async Task<WorkflowStepResponse> UpdateAsync(int id, UpdateWorkflowStepRequest request)
        {
            var existing = await _unitOfWork.WorkflowSteps.GetEntityByIdAsync(id);
            if (existing == null)
            {
                throw new Exception($"Workflow step with id {id} not found.");
            }

            _mapper.Map(request, existing);
            await _unitOfWork.WorkflowSteps.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<WorkflowStepResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.WorkflowSteps.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
            }
            return result;
        }
    }
}
