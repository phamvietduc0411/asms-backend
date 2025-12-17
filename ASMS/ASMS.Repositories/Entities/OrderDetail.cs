using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public string? OrderCode { get; set; }

    public string? StorageCode { get; set; }

    public string? ContainerCode { get; set; }

    public decimal? Price { get; set; }

    public string? Quantity { get; set; }

    public decimal? SubTotal { get; set; }

    public string? Image { get; set; }

    public int? ContainerType { get; set; }

    public int? ContainerQuantity { get; set; }

    public int? StorageTypeId { get; set; }

    public int? ShelfTypeId { get; set; }

    public int? ShelfQuantity { get; set; }

    public bool? IsPlaced { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string? ShortCode { get; set; }

    public virtual Container? ContainerCodeNavigation { get; set; }

    public virtual Order? OrderCodeNavigation { get; set; }

    public virtual ICollection<OrderDetailProductType> OrderDetailProductTypes { get; set; } = new List<OrderDetailProductType>();

    public virtual ICollection<OrderDetailService> OrderDetailServices { get; set; } = new List<OrderDetailService>();

    public virtual Storage? StorageCodeNavigation { get; set; }
}
