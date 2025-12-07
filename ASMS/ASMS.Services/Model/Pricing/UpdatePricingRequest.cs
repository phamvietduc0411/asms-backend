using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Pricing
{
    public class UpdatePricingRequest
    {
        public int PricingId { get; set; }
        public string ServiceType { get; set; } = null!;
        public string? ItemCode { get; set; }
        public bool? HasAirConditioning { get; set; }
        public decimal? PricePerMonth { get; set; }
        public decimal? PricePerWeek { get; set; }
        public decimal? PricePerTrip { get; set; }
        public string? AdditionalInfo { get; set; }
        public bool? IsActive { get; set; }
    }
}
