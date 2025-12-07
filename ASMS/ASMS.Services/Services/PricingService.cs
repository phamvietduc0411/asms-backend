using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Pricing;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PricingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ==================== PRICING CRUD ====================

        public async Task<IEnumerable<PricingResponse>> GetAllPricingAsync()
        {
            var entities = await _unitOfWork.Pricings.GetAllAsync();
            return _mapper.Map<IEnumerable<PricingResponse>>(entities);
        }

        public async Task<PricingResponse?> GetPricingByIdAsync(int pricingId)
        {
            var entity = await _unitOfWork.Pricings.GetByIdAsync(pricingId);
            return entity == null ? null : _mapper.Map<PricingResponse>(entity);
        }

        public async Task<IEnumerable<PricingResponse>> GetPricingByServiceTypeAsync(string serviceType)
        {
            var entities = await _unitOfWork.Pricings.GetByServiceTypeAsync(serviceType);
            return _mapper.Map<IEnumerable<PricingResponse>>(entities);
        }

        public async Task<PricingResponse> CreatePricingAsync(CreatePricingRequest request)
        {
            var entity = _mapper.Map<Pricing>(request);
            entity.CreatedDate = DateTime.Now;
            entity.UpdatedDate = DateTime.Now;
            entity.IsActive = true;

            await _unitOfWork.Pricings.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<PricingResponse>(entity);
        }

        public async Task<PricingResponse?> UpdatePricingAsync(UpdatePricingRequest request)
        {
            var entity = await _unitOfWork.Pricings.GetByIdAsync(request.PricingId);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            entity.UpdatedDate = DateTime.Now;

            await _unitOfWork.Pricings.UpdateAsync(entity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<PricingResponse>(entity);
        }

        public async Task<bool> DeletePricingAsync(int pricingId)
        {
            var entity = await _unitOfWork.Pricings.GetByIdAsync(pricingId);
            if (entity == null) return false;

            await _unitOfWork.Pricings.DeleteAsync(entity);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ==================== SHIPPING RATE CRUD ====================

        public async Task<IEnumerable<ShippingRateResponse>> GetAllShippingRatesAsync()
        {
            var entities = await _unitOfWork.ShippingRates.GetAllAsync();
            var responses = _mapper.Map<IEnumerable<ShippingRateResponse>>(entities).ToList();

            // Add display text for distance and quantity ranges
            foreach (var response in responses)
            {
                response.DistanceRangeDisplay = response.DistanceMaxKm.HasValue
                    ? $"{response.DistanceMinKm}-{response.DistanceMaxKm} km"
                    : $">{response.DistanceMinKm} km";

                response.ContainerQtyDisplay = response.ContainerQtyMax.HasValue
                    ? $"{response.ContainerQtyMin}-{response.ContainerQtyMax} thùng"
                    : $">{response.ContainerQtyMin} thùng";
            }

            return responses;
        }

        public async Task<ShippingRateResponse?> GetShippingRateByIdAsync(int shippingRateId)
        {
            var entity = await _unitOfWork.ShippingRates.GetByIdAsync(shippingRateId);
            if (entity == null) return null;

            var response = _mapper.Map<ShippingRateResponse>(entity);
            response.DistanceRangeDisplay = entity.DistanceMaxKm.HasValue
                ? $"{entity.DistanceMinKm}-{entity.DistanceMaxKm} km"
                : $">{entity.DistanceMinKm} km";

            response.ContainerQtyDisplay = entity.ContainerQtyMax.HasValue
                ? $"{entity.ContainerQtyMin}-{entity.ContainerQtyMax} thùng"
                : $">{entity.ContainerQtyMin} thùng";

            return response;
        }

        public async Task<ShippingRateResponse> CreateShippingRateAsync(CreateShippingRateRequest request)
        {
            var entity = _mapper.Map<ShippingRate>(request);
            entity.CreatedDate = DateTime.Now;
            entity.IsActive = true;

            await _unitOfWork.ShippingRates.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ShippingRateResponse>(entity);
        }

        public async Task<ShippingRateResponse?> UpdateShippingRateAsync(UpdateShippingRateRequest request)
        {
            var entity = await _unitOfWork.ShippingRates.GetByIdAsync(request.ShippingRateId);
            if (entity == null) return null;

            _mapper.Map(request, entity);

            await _unitOfWork.ShippingRates.UpdateAsync(entity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ShippingRateResponse>(entity);
        }

        public async Task<bool> DeleteShippingRateAsync(int shippingRateId)
        {
            var entity = await _unitOfWork.ShippingRates.GetByIdAsync(shippingRateId);
            if (entity == null) return false;

            await _unitOfWork.ShippingRates.DeleteAsync(entity);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
