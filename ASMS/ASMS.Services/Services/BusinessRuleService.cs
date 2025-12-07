using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.BusinessRule;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ASMS.Services.Services
{
    public class BusinessRuleService : IBusinessRuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessRuleService> _logger;

        public BusinessRuleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BusinessRuleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedBusinessRuleResponse> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? ruleCode)
        {
            var businessRules = await _unitOfWork.BusinessRules.GetWithFilterAsync(
                pageNumber, pageSize, ruleCode);

            var totalCount = await _unitOfWork.BusinessRules.GetTotalCountWithFilterAsync(ruleCode);

            var businessRuleResponses = businessRules.Select(br => new BusinessRuleResponse
            {
                BusinessRuleId = br.BusinessRuleId,
                RuleCode = br.RuleCode,
                Category = br.Category,
                RuleName = br.RuleName,
                RuleDescription = br.RuleDescription,
                RuleType = br.RuleType,
                Priority = br.Priority,
                IsActive = br.IsActive,
                EffectiveDate = br.EffectiveDate,
                ExpiryDate = br.ExpiryDate,
                CreatedDate = br.CreatedDate,
                UpdatedDate = br.UpdatedDate,
                CreatedBy = br.CreatedBy,
                UpdatedBy = br.UpdatedBy,
                Notes = br.Notes
            }).ToList();

            return new PaginatedBusinessRuleResponse
            {
                Data = businessRuleResponses,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<BusinessRuleResponse> CreateAsync(CreateBusinessRuleRequest request)
        {
            try
            {

                var exists = await _unitOfWork.BusinessRules.RuleCodeExistsAsync(request.RuleCode);
                if (exists)
                {
                    _logger.LogWarning("RuleCode {Code} already exists", request.RuleCode);
                    throw new ArgumentException($"RuleCode {request.RuleCode} already exists");
                }

                var businessRule = new BusinessRule
                {
                    RuleCode = request.RuleCode,
                    Category = request.Category,
                    RuleName = request.RuleName,
                    RuleDescription = request.RuleDescription,
                    RuleType = request.RuleType,
                    Priority = request.Priority,
                    IsActive = request.IsActive ?? true,
                    EffectiveDate = request.EffectiveDate,
                    ExpiryDate = request.ExpiryDate,
                    CreatedDate = DateTime.Now,
                    CreatedBy = request.CreatedBy,
                    Notes = request.Notes
                };

                await _unitOfWork.BusinessRules.AddAsync(businessRule);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created business rule {Code}", businessRule.RuleCode);

                return new BusinessRuleResponse
                {
                    BusinessRuleId = businessRule.BusinessRuleId,
                    RuleCode = businessRule.RuleCode,
                    Category = businessRule.Category,
                    RuleName = businessRule.RuleName,
                    RuleDescription = businessRule.RuleDescription,
                    RuleType = businessRule.RuleType,
                    Priority = businessRule.Priority,
                    IsActive = businessRule.IsActive,
                    EffectiveDate = businessRule.EffectiveDate,
                    ExpiryDate = businessRule.ExpiryDate,
                    CreatedDate = businessRule.CreatedDate,
                    UpdatedDate = businessRule.UpdatedDate,
                    CreatedBy = businessRule.CreatedBy,
                    UpdatedBy = businessRule.UpdatedBy,
                    Notes = businessRule.Notes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating business rule");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int businessRuleId, UpdateBusinessRuleRequest request)
        {
            try
            {
                var businessRule = await _unitOfWork.BusinessRules.GetEntityByIdAsync(businessRuleId);

                if (businessRule == null)
                {
                    _logger.LogWarning("BusinessRule {Id} not found", businessRuleId);
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(request.Category))
                    businessRule.Category = request.Category;

                if (!string.IsNullOrWhiteSpace(request.RuleName))
                    businessRule.RuleName = request.RuleName;

                if (!string.IsNullOrWhiteSpace(request.RuleDescription))
                    businessRule.RuleDescription = request.RuleDescription;

                if (!string.IsNullOrWhiteSpace(request.RuleType))
                    businessRule.RuleType = request.RuleType;

                if (!string.IsNullOrWhiteSpace(request.Priority))
                    businessRule.Priority = request.Priority;

                if (request.IsActive.HasValue)
                    businessRule.IsActive = request.IsActive.Value;

                if (request.EffectiveDate.HasValue)
                    businessRule.EffectiveDate = request.EffectiveDate;

                if (request.ExpiryDate.HasValue)
                    businessRule.ExpiryDate = request.ExpiryDate;

                if (!string.IsNullOrWhiteSpace(request.Notes))
                    businessRule.Notes = request.Notes;

                businessRule.UpdatedDate = DateTime.Now;
                businessRule.UpdatedBy = request.UpdatedBy;

                await _unitOfWork.BusinessRules.UpdateAsync(businessRule);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated business rule {Id}", businessRuleId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating business rule {Id}", businessRuleId);
                throw;
            }
        }
    }
}
