using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Pricing
{
    public class GetPriceRequest
    {
        public string ServiceType { get; set; } = null!; 
        public string ItemCode { get; set; } = null!;
        public bool? HasAirConditioning { get; set; }
        public string PriceType { get; set; } = "Month"; 
    }
}
