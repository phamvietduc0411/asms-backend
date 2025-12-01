using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Distance
{
    public class DistanceCalculationRequest
    {
        [Required(ErrorMessage = "Origin address is required")]
        public string Origin { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination address is required")]
        public string Destination { get; set; } = string.Empty;
    }
}
