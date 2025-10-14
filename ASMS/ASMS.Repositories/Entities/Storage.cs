using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Storage
{
    public string StorageCode { get; set; } = null!;

    public string? BuildingCode { get; set; }

    public int? StorageTypeId { get; set; }

    public int? ProductTypeId { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual Building? BuildingCodeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductType? ProductType { get; set; }

    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();

    public virtual ICollection<StorageBlock> StorageBlocks { get; set; } = new List<StorageBlock>();

    public virtual StorageType? StorageType { get; set; }
}
