using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.BusinessRule
{
    public class BusinessRuleResponse
    {
        public int BusinessRuleId { get; set; }
        public string RuleCode { get; set; }
        public string? Category { get; set; }
        public string RuleName { get; set; }
        public string RuleDescription { get; set; }
        public string? RuleType { get; set; }
        public string? Priority { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Notes { get; set; }
    }
}
