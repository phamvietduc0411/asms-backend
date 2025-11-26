using ASMS.Services.Interfaces;
using ASMS.Services.Model.CLP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CLPController : ControllerBase
    {
        private readonly ICLPService _clpService;

        public CLPController(ICLPService clpService)
        {
            _clpService = clpService;
        }
        /// <summary>
        /// Preview: Tìm Container phù hợp (không lưu DB)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/CLP/preview-containers
        ///     {
        ///         "productTypeID": 1,
        ///         "packageLength": 0.45,
        ///         "packageWidth": 0.45,
        ///         "packageHeight": 0.40,
        ///         "packageWeight": 15.0,
        ///         "storageDays": 30,
        ///         "isFragile": true
        ///     }
        /// 
        /// </remarks>
        [HttpPost("find-suitable-containers")]
        public async Task<ActionResult<List<ContainerPlacementDto>>> FindSuitableContainers(
    [FromBody] FindContainerRequest request)
        {
            try
            {
                // Gọi CLP Service
                var placements = await _clpService.FindSuitableContainersAsync(request);

                if (!placements.Any())
                {
                    return NotFound("No suitable containers found");
                }

                return Ok(placements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
