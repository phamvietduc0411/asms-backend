using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.Services;

namespace ASMS.Services.Interfaces
{
    public interface IServiceService
    {
        Task<PaginatedServiceResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains, decimal? minPrice, decimal? maxPrice);
        Task<ServiceResponse> CreateAsync(CreateServiceRequest request);
        Task<ServiceResponse> UpdateAsync(int id, UpdateServiceRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
