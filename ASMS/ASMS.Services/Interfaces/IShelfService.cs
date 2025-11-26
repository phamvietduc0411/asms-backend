using ASMS.Repositories.Common;
using ASMS.Services.Model.Shelves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IShelfService
    {
        Task<PaginatedList<ShelfResponse>> GetWithFilterAsync(string? storageCode, int pageNumber, int pageSize);
        Task<ShelfResponse?> GetByCodeAsync(string shelfCode);
        Task<ShelfResponse> CreateAsync(CreateShelfRequest request);
        Task<ShelfResponse?> UpdateAsync(string shelfCode, UpdateShelfRequest request);
        Task<bool> DeleteAsync(string shelfCode);
    }
}
