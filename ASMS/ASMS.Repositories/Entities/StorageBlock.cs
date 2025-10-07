using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class StorageBlock
{
    public string StorageBlockCode { get; set; } = null!;

    public string? StorageCode { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual Storage? StorageCodeNavigation { get; set; }
}
