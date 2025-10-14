using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Employee
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

    public bool? IsActive { get; set; }

    public virtual Building? BuildingCodeNavigation { get; set; }

    public virtual EmployeeRole? EmployeeRole { get; set; }
}
