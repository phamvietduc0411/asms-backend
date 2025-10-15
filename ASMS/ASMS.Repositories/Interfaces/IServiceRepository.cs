using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task<List<Service>> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains, decimal? minPrice, decimal? maxPrice);
        Task<int> GetTotalCountWithFilterAsync(string? nameContains, decimal? minPrice, decimal? maxPrice);
        Task<bool> DeleteAsync(int id);
    }
}
