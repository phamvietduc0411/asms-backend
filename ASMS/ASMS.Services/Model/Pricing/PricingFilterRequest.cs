using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Pricing
{
    public class PricingFilterRequest
    {
        public string? ServiceType { get; set; }
        public string? ItemCode { get; set; }
        public bool? HasAirConditioning { get; set; }
        public bool? IsActive { get; set; }
    }
}
