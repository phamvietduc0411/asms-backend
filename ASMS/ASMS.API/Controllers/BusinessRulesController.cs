using ASMS.Services.Interfaces;
using ASMS.Services.Model.BusinessRule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessRulesController : ControllerBase
    {
        private readonly IBusinessRuleService _businessRuleService;

        public BusinessRulesController(IBusinessRuleService businessRuleService)
        {
            _businessRuleService = businessRuleService;
        }

        /// <summary>
        /// Get business rules with pagination and optional filter by RuleCode
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWithFilter(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? ruleCode = null)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than 0");

            var result = await _businessRuleService.GetWithFilterAsync(
                pageNumber, pageSize, ruleCode);

            return Ok(result);
        }

        /// <summary>
        /// Create a new business rule
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBusinessRuleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RuleCode))
                return BadRequest("RuleCode is required");

            if (string.IsNullOrWhiteSpace(request.RuleName))
                return BadRequest("RuleName is required");

            if (string.IsNullOrWhiteSpace(request.RuleDescription))
                return BadRequest("RuleDescription is required");

            try
            {
                var businessRule = await _businessRuleService.CreateAsync(request);
                return Ok(new
                {
                    message = "Business rule created successfully",
                    data = businessRule
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing business rule
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBusinessRuleRequest request)
        {
            try
            {
                var result = await _businessRuleService.UpdateAsync(id, request);

                if (!result)
                    return NotFound($"Business rule with ID {id} not found");

                return Ok(new { message = "Business rule updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
