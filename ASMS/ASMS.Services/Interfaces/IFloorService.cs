using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Services.Model.Floor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IFloorService
    {
        Task<PaginatedList<FloorResponse>> GetWithFilterAsync(string? shelfCode, int pageNumber, int pageSize);
        Task<FloorResponse?> GetByCodeAsync(string floorCode);
        Task<FloorResponse> CreateAsync(CreateFloorRequest request);
        Task<FloorResponse?> UpdateAsync(UpdateFloorRequest request);
        Task<bool> DeleteAsync(string floorCode);
    }

}
