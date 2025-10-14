using ASMS.Services.Interfaces;
using ASMS.Services.Model.Building;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingService _buildingService;
        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }
        #region CRUD Building
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var building = await _buildingService.GetByIdAsync(id);
            if (building == null)
                return NotFound(new { message = $"Building with code {id} not found." });
            return Ok(building);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateBuildingRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.BuildingCode))
                {
                    request.BuildingCode = await _buildingService.GetLastRecord();
                }
                var result = await _buildingService.AddBuildingAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ErrorMessage = ex.Message,
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateByIdAsync(int id, [FromBody] UpdateBuildingRequest newBuilding)
        {
            if (newBuilding == null)
                return BadRequest(new { message = "Invalid data." });

            var existingBuilding = await _buildingService.GetByIdAsync(id);
            if (existingBuilding == null)
                return NotFound(new { message = $"Building with id {id} not found." });
            if (string.IsNullOrWhiteSpace(newBuilding.BuildingCode))
            {
                newBuilding.BuildingCode = await _buildingService.GetLastRecord();
            }

            existingBuilding.BuildingCode = newBuilding.BuildingCode;
            existingBuilding.Name = newBuilding.Name;
            existingBuilding.Area = newBuilding.Area;
            existingBuilding.Address = newBuilding.Address;
            existingBuilding.FloorQuantity = newBuilding.FloorQuantity;
            existingBuilding.Status = newBuilding.Status;
            existingBuilding.IsActive = newBuilding.IsActive;

            var updateBuilding = await _buildingService.UpdateBuildingAsync(existingBuilding);

            if (updateBuilding == null)
                return StatusCode(500, new { message = "Failed to update building info." });

            return Ok(new
            {
                message = "Update successful.",
                data = updateBuilding
            });
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var existingBuilding = await _buildingService.GetByIdAsync(id);
            if (existingBuilding == null)
                return NotFound(new { message = "Not found" });
            existingBuilding.IsActive = false;
            var deleteRole = await _buildingService.UpdateBuildingAsync(existingBuilding);

            if (deleteRole == null)
                return StatusCode(500, new { message = "Failed to delete building." });

            return Ok(new { message = "Marked as deleted." });
        }
        #endregion
    }

}
