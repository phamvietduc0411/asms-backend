using ASMS.Repositories.Common;
using ASMS.Services.Model.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IContainerService
    {
        Task<PaginatedList<ContainerResponse>> GetWithFilterAsync(string? floorCode, string? shelfCode, string? storageCode, int pageNumber, int pageSize);
        Task<ContainerResponse?> GetByCodeAsync(string code);
        Task<ContainerResponse> CreateAsync(CreateContainerRequest request);
        Task<ContainerResponse?> UpdateAsync(string code, UpdateContainerRequest request);
        Task<bool> DeleteAsync(string code);
        Task<bool> UpdateContainerPositionAsync(UpdateContainerPositionRequest request);
        Task<PlaceContainerResponse> PlaceContainerAsync(PlaceContainerRequest request);
        Task<RemoveContainerResponse> RemoveContainerAsync(string containerCode, string orderCode, string performedBy);
        Task<bool> UpdateContainerPositionSerialNumberAsync(string containerCode, int? serialNumber, int? layer);
    }
}
