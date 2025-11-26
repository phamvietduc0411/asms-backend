using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.EmployeeRole
{
    public class GetEmployeeRoleResponse
    {
        public int EmployeeRoleId { get; set; }

        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
