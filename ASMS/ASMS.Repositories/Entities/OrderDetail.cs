using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public string? OrderCode { get; set; }

    public string? StorageCode { get; set; }

    public string? ContainerCode { get; set; }

    public int? ServiceId { get; set; }

    public decimal? Price { get; set; }

    public string? Quantity { get; set; }

    public decimal? SubTotal { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    public virtual Container? ContainerCodeNavigation { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual Order? OrderCodeNavigation { get; set; }

    public virtual Service? Service { get; set; }

    public virtual Storage? StorageCodeNavigation { get; set; }
}
