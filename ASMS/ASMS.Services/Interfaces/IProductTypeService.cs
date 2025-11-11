using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Services.Model;
using ASMS.Services.Model.ProductType;
using ASMS.Services.Services;

namespace ASMS.Services.Interfaces
{
    public interface IProductTypeService
    {
        Task<ProductType?> GetByIdAsync(int id);
        Task<ProductType> AddProductTypeAsync(CreateTypeRequest newType);
        Task<ProductType> UpdateProductTypeAsync(ProductType productType);
        Task<PaginatedList<GetProductTypeResponse>> GetWithFilterAsync(bool? isActive, int pageNumber, int pageSize);
    }
}
