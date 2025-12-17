using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ShortLink
{
    public int Id { get; set; }

    public string ShortCode { get; set; } = null!;

    public string OriginalUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int ClickCount { get; set; }
}
