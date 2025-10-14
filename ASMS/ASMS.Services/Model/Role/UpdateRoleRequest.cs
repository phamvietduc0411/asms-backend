using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Role
{
    public class UpdateRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}
