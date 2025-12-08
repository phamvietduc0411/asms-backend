using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Contact
{
    public class UpdateContactRequest
    {
        public string? CustomerCode { get; set; }
        public string? EmployeeCode { get; set; }
        public string? OrderCode { get; set; }
        public string? Name { get; set; }
        public string? PhoneContact { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public bool? IsActive { get; set; }
    }
}
