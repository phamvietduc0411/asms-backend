using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.BusinessRule;

namespace ASMS.Services.Interfaces
{
    public interface IBusinessRuleService
    {
        Task<PaginatedBusinessRuleResponse> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? ruleCode);

        Task<BusinessRuleResponse> CreateAsync(CreateBusinessRuleRequest request);
        Task<bool> UpdateAsync(int businessRuleId, UpdateBusinessRuleRequest request);
    }
}
