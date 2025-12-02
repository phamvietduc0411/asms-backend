using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Services
{
    public class ServiceResponse
    {
        public int ServiceId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Vname { get; set; }
    }
}
