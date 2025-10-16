using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Customer
{
    public class UpdateCustomerRequest
    {
        public string CustomerCode { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Name { get; set; }

        public bool? IsActive { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
