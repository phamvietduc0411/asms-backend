using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Services
{
    public class CreateServiceRequest
    {
        [Required]
        public int ServiceId { get; set; } 

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        public string? Description { get; set; }
        public string? Vname { get; set; }
    }
}
