using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Customer
{
    public int Id { get; set; }
    public string CustomerCode { get; set; } = null!;

    public string? Phone { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public string? Address { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
