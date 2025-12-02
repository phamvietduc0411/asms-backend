using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ProductType
{
    public class GetProductTypeResponse
    {
        public int ProductTypeId { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFragile { get; set; }
        public bool? CanStack { get; set; }
        public string? Description { get; set; }
        public string? Vname { get; set; }
    }
}
