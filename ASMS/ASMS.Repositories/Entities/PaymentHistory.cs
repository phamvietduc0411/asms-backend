using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class PaymentHistory
{
    public string PaymentHistoryCode { get; set; } = null!;

    public string? OrderCode { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentPlatform { get; set; }

    public decimal? Amount { get; set; }

    public virtual Order? OrderCodeNavigation { get; set; }
}
