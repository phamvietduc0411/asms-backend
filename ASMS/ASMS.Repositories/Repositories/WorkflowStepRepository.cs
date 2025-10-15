using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class WorkflowStepRepository : GenericRepository<WorkflowStep>, IWorkflowStepRepository
    {
        public WorkflowStepRepository(
            VstorageContext context,
            ILogger logger) : base(context, logger)
        {
        }
        public async Task<List<WorkflowStep>> GetWithFilterAsync(int pageNumber, int pageSize, string? workflowTemplateName)
        {
            try
            {
                var query = _dbSet
                    .Include(ws => ws.WorkflowTemplate)
                    .AsQueryable();
                if (!string.IsNullOrWhiteSpace(workflowTemplateName))
                {
                    query = query.Where(ws => ws.WorkflowTemplate != null
                                           && ws.WorkflowTemplate.Name != null
                                           && ws.WorkflowTemplate.Name.Contains(workflowTemplateName));
                }
                return await query
                    .OrderBy(ws => ws.WorkflowTemplateId)
                    .ThenBy(ws => ws.StepNumber)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error occurred while getting workflow steps with filter.");
                throw;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var workflowStep = await _dbSet.FindAsync(id);
                if (workflowStep == null)
                {
                    return false;
                }

                _dbSet.Remove(workflowStep);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting workflow step with id: {id}");
                throw;
            }
        }
        public async Task<int> GetTotalCountWithFilterAsync(string? workflowTemplateName)
        {
            try
            {
                var query = _dbSet
                    .Include(ws => ws.WorkflowTemplate)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(workflowTemplateName))
                {
                    query = query.Where(ws => ws.WorkflowTemplate != null
                                           && ws.WorkflowTemplate.Name != null
                                           && ws.WorkflowTemplate.Name.Contains(workflowTemplateName));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting total count with filter.");
                throw;
            }
        }
    }
}
