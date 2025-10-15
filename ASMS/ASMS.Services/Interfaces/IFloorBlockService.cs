using ASMS.Services.Model.FloorBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IFloorBlockService
    {
        Task<IEnumerable<FloorBlockResponse>> GetAllAsync();
        Task<FloorBlockResponse?> GetByCodeAsync(string floorBlockCode);
        Task<FloorBlockResponse> CreateAsync(CreateFloorBlockRequest request);
        Task<FloorBlockResponse?> UpdateAsync(string floorBlockCode, UpdateFloorBlockRequest request);
        Task<bool> DeleteAsync(string floorBlockCode);
    }
}
