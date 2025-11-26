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
    public class PaymentResultRepository : GenericRepository<PaymentResult>, IPaymentResultRepository
    {
        public PaymentResultRepository(VstorageContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public async Task<PaymentResult?> GetByPaymentCodeAsync(string paymentCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.PaymentCode == paymentCode);
        }
    }
}
