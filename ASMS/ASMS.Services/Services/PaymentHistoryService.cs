using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.PaymentHistory;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class PaymentHistoryService : IPaymentHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentHistoryResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.PaymentHistories.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentHistoryResponse>>(entities);
        }

        public async Task<PaymentHistoryResponse?> GetByCodeAsync(string code)
        {
            var entity = await _unitOfWork.PaymentHistories.GetByCodeAsync(code);
            return entity == null ? null : _mapper.Map<PaymentHistoryResponse>(entity);
        }

        public async Task<PaymentHistoryResponse> CreateAsync(CreatePaymentHistoryRequest request)
        {
            var entity = _mapper.Map<PaymentHistory>(request);
            await _unitOfWork.PaymentHistories.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PaymentHistoryResponse>(entity);
        }

        public async Task<PaymentHistoryResponse?> UpdateAsync(string code, UpdatePaymentHistoryRequest request)
        {
            var entity = await _unitOfWork.PaymentHistories.GetByCodeAsync(code);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            await _unitOfWork.PaymentHistories.UpdateAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PaymentHistoryResponse>(entity);
        }

        //public async Task<bool> DeleteAsync(string code)
        //{
        //    var entity = await _unitOfWork.PaymentHistories.GetByCodeAsync(code);
        //    if (entity == null) return false;

        //    await _unitOfWork.PaymentHistories.DeleteAsync(code);

        //    await _unitOfWork.CompleteAsync();
        //    return true;
        //}
    }
}
