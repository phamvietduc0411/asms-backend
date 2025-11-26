using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class StorageType
{
    public int StorageTypeId { get; set; }

    public string? Name { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public decimal? TotalVolume { get; set; }

    public decimal? Area { get; set; }

    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
