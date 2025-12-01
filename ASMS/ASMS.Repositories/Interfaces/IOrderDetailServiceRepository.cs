using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IOrderDetailServiceRepository : IGenericRepository<OrderDetailService>
    {
        Task<List<Service>> GetByIdsAsync(List<int> ids);
        Task<IEnumerable<OrderDetailService>> GetByOrderDetailIdAsync(int orderDetailId);
    }
}
