using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Services.Model.Building;

namespace ASMS.Services.Interfaces
{
    public interface IBuildingService
    {
        Task<Building> GetByIdAsync(int id);
        Task<Building> AddBuildingAsync(CreateBuildingRequest request);
        Task<Building> UpdateBuildingAsync(Building building);
        Task<string> GetLastRecord();
        Task<PaginatedList<GetBuildingResponse>> GetAllAsync(int pageNumber, int pageSize);
    }
}
