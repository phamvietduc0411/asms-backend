using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Common;
using ASMS.Services.Model.ShelfType;

namespace ASMS.Services.Interfaces
{
    public interface IShelfTypeService
    {
        Task<PaginatedList<GetShelfTypeResponse>> GetAllAsync(int pageNumber, int pageSize);
        Task<GetShelfTypeResponse> GetByIdAsync(int id);
        Task<GetShelfTypeResponse> CreateAsync(CreateShelfTypeRequest request);
        Task<GetShelfTypeResponse> UpdateAsync(int id, UpdateShelfTypeRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
