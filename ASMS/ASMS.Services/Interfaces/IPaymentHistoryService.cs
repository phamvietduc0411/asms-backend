using ASMS.Services.Model.PaymentHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IPaymentHistoryService
    {
        Task<IEnumerable<PaymentHistoryResponse>> GetAllAsync();
        Task<PaymentHistoryResponse?> GetByCodeAsync(string code);
        Task<PaymentHistoryResponse> CreateAsync(CreatePaymentHistoryRequest request);
        Task<PaymentHistoryResponse?> UpdateAsync(string code, UpdatePaymentHistoryRequest request);
        //Task<bool> DeleteAsync(string code);
    }
}
