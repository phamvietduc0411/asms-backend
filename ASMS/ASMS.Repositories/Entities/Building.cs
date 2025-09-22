using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Building
{
    public string BuildingCode { get; set; } = null!;

    public string? BuildingName { get; set; }

    public string? Area { get; set; }

    public string? Address { get; set; }

    public int? FloorQuantity { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
