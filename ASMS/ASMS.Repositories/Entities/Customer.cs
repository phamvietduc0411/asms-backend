using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Customer
{
    public string CustomerCode { get; set; } = null!;

    public string? Phone { get; set; }

    public string? CustomerName { get; set; }

    public bool? IsActive { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
