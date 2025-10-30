using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class EmployeeRole
{
    public int EmployeeRoleId { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
