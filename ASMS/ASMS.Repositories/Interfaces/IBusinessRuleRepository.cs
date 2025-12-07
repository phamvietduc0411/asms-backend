using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IBusinessRuleRepository : IGenericRepository<BusinessRule>
    {
        Task<List<BusinessRule>> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? ruleCode);

        Task<int> GetTotalCountWithFilterAsync(string? ruleCode);
        Task<BusinessRule?> GetByRuleCodeAsync(string ruleCode);
        Task<bool> RuleCodeExistsAsync(string ruleCode);
    }
}
