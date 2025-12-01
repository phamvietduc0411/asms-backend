using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Repositories
{
    public class PaymentHistoryRepository : GenericRepository<PaymentHistory>, IPaymentHistoryRepository
    {
        public PaymentHistoryRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<PaymentHistory>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<PaymentHistory?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.PaymentHistoryCode == code);
        }

        public async Task DeleteAsync(PaymentHistory entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<List<PaymentHistory>> GetHistoryByCustomerCode(string customerCode, string? orderCode)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(ph => ph.OrderCodeNavigation) 
                .Where(ph => ph.OrderCodeNavigation != null
                             && ph.OrderCodeNavigation.CustomerCode == customerCode);

            if (!string.IsNullOrWhiteSpace(orderCode))
            {
                query = query.Where(ph => ph.OrderCode == orderCode);
            }

            return await query.ToListAsync();
        }
    }
}
