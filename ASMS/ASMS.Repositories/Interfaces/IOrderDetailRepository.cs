using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        Task<PaginatedList<OrderDetail>> GetWithFilterAsync(bool? isPlaced, string? orderCode, int pageNumber, int pageSize);
        Task<OrderDetail?> GetByIdAsync(int id);
        Task<List<OrderDetail>> GetByOrderCodeAsync(string orderCode);
        Task<int> GetMaxOrderDetailIdAsync();
    }

}
