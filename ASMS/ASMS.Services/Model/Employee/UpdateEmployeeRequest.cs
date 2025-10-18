using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Employee
{
    public class UpdateEmployeeRequest
    {
        public string EmployeeCode { get; set; } = null!;

        public int? EmployeeRoleId { get; set; }

        public string? Name { get; set; }

        public int? BuildingId { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Status { get; set; }

        public bool IsActive { get; set; }
    }
}
