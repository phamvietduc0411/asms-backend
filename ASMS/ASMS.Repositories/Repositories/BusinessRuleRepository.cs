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
    public class BusinessRuleRepository : GenericRepository<BusinessRule>, IBusinessRuleRepository
    {
        public BusinessRuleRepository(VstorageContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public async Task<List<BusinessRule>> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? ruleCode)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(ruleCode))
                    query = query.Where(br => br.RuleCode == ruleCode);

                return await query
                    .OrderByDescending(br => br.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business rules with filter");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(string? ruleCode)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(ruleCode))
                    query = query.Where(br => br.RuleCode == ruleCode);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total count with filter");
                throw;
            }
        }
        public async Task<BusinessRule?> GetByRuleCodeAsync(string ruleCode)
        {
            try
            {
                return await _dbSet
                    .FirstOrDefaultAsync(br => br.RuleCode == ruleCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business rule by code {Code}", ruleCode);
                throw;
            }
        }
        public async Task<bool> RuleCodeExistsAsync(string ruleCode)
        {
            try
            {
                return await _dbSet.AnyAsync(br => br.RuleCode == ruleCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking rule code existence");
                throw;
            }
        }

    }
}
