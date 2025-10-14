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
    public class WorkflowTemplateRepository : GenericRepository<WorkflowTemplate>, IWorkflowTemplateRepository
    {
        public WorkflowTemplateRepository(
            VstorageContext context,
            ILogger logger) : base(context, logger)
        {
        }
        public async Task<List<WorkflowTemplate>> GetWithFilterAsync(int pageNumber, int pageSize, int? storageTypeId, string? statusContains)
        {
            try
            {
                var query = _dbSet
                    .Include(w => w.StorageType)
                    .AsQueryable();

                if (storageTypeId.HasValue)
                {
                    query = query.Where(w => w.StorageTypeId == storageTypeId.Value);
                }

                if (!string.IsNullOrWhiteSpace(statusContains))
                {
                    query = query.Where(w => w.Status != null && w.Status.Contains(statusContains));
                }

                return await query
                    .OrderBy(w => w.WorkflowTemplateId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting workflow templates with filter.");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(int? storageTypeId, string? statusContains)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (storageTypeId.HasValue)
                {
                    query = query.Where(w => w.StorageTypeId == storageTypeId.Value);
                }

                if (!string.IsNullOrWhiteSpace(statusContains))
                {
                    query = query.Where(w => w.Status != null && w.Status.Contains(statusContains));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting total count with filter.");
                throw;
            }
        }

        public async Task<WorkflowTemplate?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(w => w.StorageType)
                    .Include(w => w.WorkflowSteps)
                    .FirstOrDefaultAsync(w => w.WorkflowTemplateId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting workflow template with id: {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var workflowTemplate = await _dbSet.FindAsync(id);
                if (workflowTemplate == null)
                {
                    return false;
                }

                _dbSet.Remove(workflowTemplate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting workflow template with id: {id}");
                throw;
            }
        }
    }
}
