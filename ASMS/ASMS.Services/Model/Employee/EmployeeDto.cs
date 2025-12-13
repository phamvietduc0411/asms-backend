using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Employee
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public int? EmployeeRoleId { get; set; }
        public string? Name { get; set; }
        public int? BuildingId { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Username { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; }
        public int? OrderActionCount { get; set; }

        public string? EmployeeRoleName { get; set; }
        public string? BuildingName { get; set; }
    }
}
