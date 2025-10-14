using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Building
{
    public int Id { get; set; }
    public string BuildingCode { get; set; } = null!;

    public string? Name { get; set; }

    public string? Area { get; set; }

    public string? Address { get; set; }

    public int? FloorQuantity { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
