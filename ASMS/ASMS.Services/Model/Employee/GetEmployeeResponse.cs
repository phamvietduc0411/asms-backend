using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Employee
{
    public class GetEmployeeResponse
    {
        public string EmployeeCode { get; set; } = null!;
        public string? Name { get; set; }
        public int? BuildingId { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Username { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public string? RoleName { get; set; }
    }
}
