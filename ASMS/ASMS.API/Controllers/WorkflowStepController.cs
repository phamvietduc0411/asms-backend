using ASMS.Services.Interfaces;
using ASMS.Services.Model.WorkflowSteps;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowStepController : ControllerBase
    {
        private readonly IWorkflowStepService _workflowStepService;

        public WorkflowStepController(IWorkflowStepService workflowStepService)
        {
            _workflowStepService = workflowStepService;
        }

        #region Get Workflow Steps with Filter
        /// <summary>
        /// Get workflow steps with pagination and optional filter by workflow template name.
        /// </summary>
        /// <param name="pageNumber">Current page number (default = 1)</param>
        /// <param name="pageSize">Number of items per page (default = 10)</param>
        /// <param name="workflowTemplateName">Filter by Workflow Template Name contains text (optional)</param>
        /// <returns>A paginated list of workflow steps</returns>
        /// <response code="200">Returns a paginated list of workflow steps</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? workflowTemplateName = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var result = await _workflowStepService.GetWithFilterAsync(pageNumber, pageSize, workflowTemplateName);
            return Ok(result);
        }
        #endregion

        #region Create Workflow Step
        /// <summary>
        /// Create a new workflow step.
        /// </summary>
        /// <param name="request">The workflow step details to create</param>
        /// <returns>The created workflow step</returns>
        /// <response code="201">Workflow step created successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateWorkflowStepRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _workflowStepService.CreateAsync(request);
                return Created($"/api/WorkflowStep/{result.WorkflowStepId}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region Update Workflow Step
        /// <summary>
        /// Update an existing workflow step.
        /// </summary>
        /// <param name="id">The ID of the workflow step to update</param>
        /// <param name="request">The updated workflow step details</param>
        /// <returns>The updated workflow step</returns>
        /// <response code="200">Workflow step updated successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Workflow step not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateWorkflowStepRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _workflowStepService.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        #endregion

        #region Delete Workflow Step
        /// <summary>
        /// Delete a workflow step by its ID.
        /// </summary>
        /// <param name="id">The ID of the workflow step</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Workflow step deleted successfully</response>
        /// <response code="404">Workflow step not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _workflowStepService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Workflow step with id {id} not found.");
            }
            return Ok(result);
        }
        #endregion
    }
}
