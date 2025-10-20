using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.StorageTypes;

namespace ASMS.Services.Interfaces
{
    public interface IStorageTypeService
    {
        Task<PaginatedStorageTypeResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains);
        Task<StorageTypeResponse?> GetByIdAsync(int id);
        Task<StorageTypeResponse> CreateAsync(CreateStorageTypeRequest request);
        Task<StorageTypeResponse> UpdateAsync(int id, UpdateStorageTypeRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
