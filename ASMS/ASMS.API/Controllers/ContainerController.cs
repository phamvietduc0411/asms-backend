using ASMS.Services.Interfaces;
using ASMS.Services.Model.Container;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContainerController : ControllerBase
    {
        private readonly IContainerService _containerService;

        public ContainerController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        /// <summary>
        /// Retrieves all containers with optional filters (Floor, Shelf, Storage) and pagination
        /// </summary>
        /// <param name="floorCode">Optional filter by floor code</param>
        /// <param name="shelfCode">Optional filter by shelf code</param>
        /// <param name="storageCode">Optional filter by storage code</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of containers with Type from ContainerType</returns>
        [HttpGet]
        public async Task<IActionResult> GetContainers(
            [FromQuery] string? floorCode,
            [FromQuery] string? shelfCode,
            [FromQuery] string? storageCode,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _containerService.GetWithFilterAsync(floorCode, shelfCode, storageCode, pageNumber, pageSize);

                return Ok(new
                {
                    success = true,
                    data = result.Items,
                    pagination = new
                    {
                        currentPage = result.CurrentPage,
                        pageSize = result.PageSize,
                        totalRecords = result.TotalRecords,
                        totalPages = result.TotalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _containerService.GetByCodeAsync(code);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContainerRequest request)
        {
            var result = await _containerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetByCode), new { code = result.ContainerCode }, result);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update(string code, [FromBody] UpdateContainerRequest request)
        {
            var result = await _containerService.UpdateAsync(code, request);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var deleted = await _containerService.DeleteAsync(code);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        // PUT: api/containers/positions
        [HttpPut("positions")]
        public async Task<IActionResult> UpdatePositions([FromBody] UpdateContainerPositionRequest request)
        {
            try
            {
                var result = await _containerService.UpdateContainerPositionAsync(request);

                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Một hoặc nhiều containers không tìm thấy hoặc cập nhật thất bại"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Container positions updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        /// <summary>
        /// Xếp container vào vị trí đã chọn
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/container/place
        ///     {
        ///         "containerCode": "CONT-A-001",
        ///         "floorCode": "FL-1-SHELF-1",
        ///         "layer": 0,
        ///         "serialNumber": 5,
        ///         "productTypeId": 1,
        ///         "requiresRearrangement": false,
        ///         "rearrangeContainerCode": null
        ///     }
        ///     
        /// Sample request with rearrangement:
        /// 
        ///     POST /api/container/place
        ///     {
        ///         "containerCode": "CONT-B-002",
        ///         "floorCode": "FL-2-SHELF-1",
        ///         "layer": 0,
        ///         "serialNumber": 8,
        ///         "productTypeId": 2,
        ///         "requiresRearrangement": true,
        ///         "rearrangeContainerCode": "CONT-FRAGILE-A"
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">Thông tin container và vị trí</param>
        /// <returns>Kết quả xếp container</returns>
        [HttpPost("place")]
        public async Task<ActionResult<PlaceContainerResponse>> PlaceContainer(
            [FromBody] PlaceContainerRequest request)
        {
            try
            {
                var response = await _containerService.PlaceContainerAsync(request);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PlaceContainerResponse
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Lấy container ra khỏi kho (khi khách hàng lấy hàng)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/container/remove
        ///     {
        ///         "containerCode": "CONT-A-001",
        ///         "orderCode": "ORD-001",      // Optional
        ///         "performedBy": "user123"     // Optional
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">Thông tin container cần lấy ra</param>
        /// <returns>Kết quả lấy container</returns>
        [HttpPost("remove")]
        public async Task<ActionResult<RemoveContainerResponse>> RemoveContainer(
            [FromBody] RemoveContainerRequest request)  
        {
            try
            {
                if (string.IsNullOrEmpty(request.ContainerCode))
                {
                    return BadRequest(new RemoveContainerResponse
                    {
                        Success = false,
                        Message = "Container code is required"
                    });
                }

                var response = await _containerService.RemoveContainerAsync(
                    request.ContainerCode,
                    request.OrderCode,
                    request.PerformedBy);

                if (!response.Success)
                {
                    if (!string.IsNullOrEmpty(response.BlockingContainerCode))
                    {
                        return Conflict(response);
                    }
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RemoveContainerResponse
                {
                    Success = false,
                    Message = "Internal server error"
                });
            }
        }
        /// <summary>
        /// Update container position (Serial Number and/or Layer).
        /// </summary>
        /// <param name="containerCode">Container code to update</param>
        /// <param name="serialNumber">Serial Number (optional)</param>
        /// <param name="layer">Layer (optional)</param>
        [HttpPatch("{containerCode}/position")]
        public async Task<IActionResult> UpdateContainerPosition(
            string containerCode,
            [FromQuery] int? serialNumber = null,
            [FromQuery] int? layer = null)
        {
            if (string.IsNullOrWhiteSpace(containerCode))
                return BadRequest("Mã container không được để trống");

            if (!serialNumber.HasValue && !layer.HasValue)
                return BadRequest("Phải cung cấp ít nhất một trong hai: serialNumber hoặc layer");

            var result = await _containerService.UpdateContainerPositionSerialNumberAsync(containerCode, serialNumber, layer);

            if (!result)
                return NotFound($"Không tìm thấy container với mã: {containerCode}");

            return Ok(new { message = "Cập nhật vị trí container thành công" });
        }
    }
}
