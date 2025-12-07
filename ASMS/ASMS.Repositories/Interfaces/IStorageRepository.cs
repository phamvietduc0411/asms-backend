using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IStorageRepository : IGenericRepository<Storage>
    {
        Task<List<Storage>> GetWithFilterAsync(int pageNumber, int pageSize, string? buildingCode, string? storageTypeName, string? productTypeName);
        Task<int> GetTotalCountWithFilterAsync(string? buildingCode, string? storageTypeName, string? productTypeName);
        Task<Storage?> GetByCodeAsync(string storageCode);
        Task<bool> HasRelatedDataAsync(string storageCode);
        Task<int> GetNumberOfStorageWithBuildingCode(int buildingId, string storageType);
        Task<List<Storage>> GetAllStorage();
        Task<Storage?> GetByCodeWithBuildingAsync(string storageCode);
        Task<Storage?> GetByCodeAsNoTrackingAsync(string storageCode);
        Task<Storage?> GetByCodeWithoutIncludesAsync(string storageCode);
        Task<List<Storage>> GetAllAsNoTrackingAsync();
    }
}
