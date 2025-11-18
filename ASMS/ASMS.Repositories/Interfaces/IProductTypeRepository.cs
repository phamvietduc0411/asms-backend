using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IProductTypeRepository
    {
        Task<ProductType?> GetEntityByIdAsync(int id);
        Task<ProductType> AddAsync(ProductType role);
        Task<ProductType> UpdateAsync(ProductType role);
        Task<PaginatedList<ProductType>> GetWithFilterAsync(bool? isActive, int pageNumber, int pageSize);
    }
}
