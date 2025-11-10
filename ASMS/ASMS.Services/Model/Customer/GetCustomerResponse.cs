using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Customer
{
    public class GetCustomerResponse
    {
        public string CustomerCode { get; set; } = null!;
        public string? Phone { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; } = null!;
        public int Id { get; set; }
    }
}
