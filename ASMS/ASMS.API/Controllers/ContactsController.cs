using ASMS.Services.Interfaces;
using ASMS.Services.Model.Contact;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Get contacts with pagination and optional filters
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWithFilter(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? customerCode = null,
            [FromQuery] string? employeeCode = null,
            [FromQuery] string? orderCode = null)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than 0");

            var result = await _contactService.GetWithFilterAsync(
                pageNumber, pageSize, customerCode, employeeCode, orderCode);

            return Ok(result);
        }

        /// <summary>
        /// Get contact by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);

            if (contact == null)
                return NotFound($"Contact with ID {id} not found");

            return Ok(contact);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required");

            try
            {
                var contact = await _contactService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = contact.ContactId }, contact);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing contact
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContactRequest request)
        {
            try
            {
                var result = await _contactService.UpdateAsync(id, request);

                if (!result)
                    return NotFound($"Contact with ID {id} not found");

                return Ok(new { message = "Contact updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Toggle contact active status (Active <-> Inactive)
        /// </summary>
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var result = await _contactService.ToggleActiveAsync(id);

                if (result == null)
                    return NotFound($"Contact with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        /// <summary>
        /// Tạo contact mới và gửi email xác nhận cho refund
        /// </summary>
        [HttpPost("with-email")]
        public async Task<ActionResult<ContactResponse>> CreateContactWithEmail([FromBody] CreateContactRequest request)
        {
            var result = await _contactService.CreateWithEmailAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.ContactId }, result);
        }
        /// <summary>
        /// Get count of "request_to_retrieve" contacts for an order
        /// </summary>
        [HttpGet("count-retrieve-requests/{orderCode}")]
        public async Task<IActionResult> CountRequestToRetrieveByOrderCode(string orderCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderCode))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "OrderCode is required"
                    });
                }

                var count = await _contactService.CountRequestToRetrieveByOrderCodeAsync(orderCode);

                return Ok(new
                {
                    success = true,
                    orderCode = orderCode,
                    requestToRetrieveCount = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
