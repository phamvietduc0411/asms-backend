using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.Pricing;

namespace ASMS.Services.Interfaces
{
    public interface IPricingService
    {
        Task<IEnumerable<PricingResponse>> GetAllPricingAsync();
        Task<PricingResponse?> GetPricingByIdAsync(int pricingId);
        Task<IEnumerable<PricingResponse>> GetPricingByServiceTypeAsync(string serviceType);
        Task<PricingResponse> CreatePricingAsync(CreatePricingRequest request);
        Task<PricingResponse?> UpdatePricingAsync(UpdatePricingRequest request);
        Task<bool> DeletePricingAsync(int pricingId);

        Task<IEnumerable<ShippingRateResponse>> GetAllShippingRatesAsync();
        Task<ShippingRateResponse?> GetShippingRateByIdAsync(int shippingRateId);
        Task<ShippingRateResponse> CreateShippingRateAsync(CreateShippingRateRequest request);
        Task<ShippingRateResponse?> UpdateShippingRateAsync(UpdateShippingRateRequest request);
        Task<bool> DeleteShippingRateAsync(int shippingRateId);
    }
}
