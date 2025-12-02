using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ProductType
{
    public class CreateTypeRequest
    {
        public string? Name { get; set; }
        public string? Vname { get; set; }

        public string? Status { get; set; }

        public bool? IsActive { get; set; }

    }
}
