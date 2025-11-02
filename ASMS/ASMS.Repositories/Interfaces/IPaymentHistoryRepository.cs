using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IPaymentHistoryRepository : IGenericRepository<PaymentHistory>
    {
        Task<IEnumerable<PaymentHistory>> GetAllAsync();
        Task<PaymentHistory?> GetByCodeAsync(string code);
        Task DeleteAsync(PaymentHistory entity);
    }
}
