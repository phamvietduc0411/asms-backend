using ASMS.Services.Interfaces;
using ASMS.Services.Model.WorkflowTemplates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowTemplateController : ControllerBase
    {
        private readonly IWorkflowTemplateService _workflowTemplateService;

        public WorkflowTemplateController(IWorkflowTemplateService workflowTemplateService)
        {
            _workflowTemplateService = workflowTemplateService;
        }

        #region Get Workflow Templates with Filter
        /// <summary>
        /// Get workflow templates with pagination and optional filters.
        /// </summary>
        /// <param name="pageNumber">Current page number (default = 1)</param>
        /// <param name="pageSize">Number of items per page (default = 10)</param>
        /// <param name="storageTypeId">Filter by Storage Type ID (optional)</param>
        /// <param name="statusContains">Filter by Status contains text (optional)</param>
        /// <returns>A paginated list of workflow templates</returns>
        /// <response code="200">Returns a paginated list of workflow templates</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? storageTypeId = null,
            [FromQuery] string? statusContains = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var result = await _workflowTemplateService.GetWithFilterAsync(pageNumber, pageSize, storageTypeId, statusContains);
            return Ok(result);
        }
        #endregion

        #region Get Workflow Template by Id
        /// <summary>
        /// Get a workflow template by its ID.
        /// </summary>
        /// <param name="id">The ID of the workflow template</param>
        /// <returns>The workflow template details</returns>
        /// <response code="200">Returns the workflow template</response>
        /// <response code="404">Workflow template not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _workflowTemplateService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound($"Workflow template with id {id} not found.");
            }
            return Ok(result);
        }
        #endregion

        #region Create Workflow Template
        /// <summary>
        /// Create a new workflow template.
        /// </summary>
        /// <param name="request">The workflow template details to create (including WorkflowTemplateId)</param>
        /// <returns>The created workflow template</returns>
        /// <response code="201">Workflow template created successfully</response>
        /// <response code="400">Invalid request or ID already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateWorkflowTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _workflowTemplateService.CreateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update Workflow Template
        /// <summary>
        /// Update an existing workflow template.
        /// </summary>
        /// <param name="id">The ID of the workflow template to update</param>
        /// <param name="request">The updated workflow template details</param>
        /// <returns>The updated workflow template</returns>
        /// <response code="200">Workflow template updated successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Workflow template not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateWorkflowTemplateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _workflowTemplateService.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region Delete Workflow Template
        /// <summary>
        /// Delete a workflow template by its ID.
        /// </summary>
        /// <param name="id">The ID of the workflow template</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Workflow template deleted successfully</response>
        /// <response code="404">Workflow template not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _workflowTemplateService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Workflow template with id {id} not found.");
            }
            return Ok(result);
        }
        #endregion
    }
}
