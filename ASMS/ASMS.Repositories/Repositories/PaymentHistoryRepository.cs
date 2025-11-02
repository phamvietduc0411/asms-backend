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
        public PaymentHistoryRepository(VstorageContext context, ILogger logger)
            : base(context, logger) { }

        public async Task<IEnumerable<PaymentHistory>> GetAllAsync()
        {
            return await _dbSet.Include(x => x.OrderCodeNavigation).ToListAsync();
        }

        public async Task<PaymentHistory?> GetByCodeAsync(string code)
        {
            return await _dbSet.Include(x => x.OrderCodeNavigation)
                               .FirstOrDefaultAsync(x => x.PaymentHistoryCode == code);
        }
    }
}
