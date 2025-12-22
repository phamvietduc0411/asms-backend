using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Order
{
    public string OrderCode { get; set; } = null!;

    public string? CustomerCode { get; set; }

    public DateOnly? OrderDate { get; set; }

    public DateOnly? DepositDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? UnpaidAmount { get; set; }

    public string? CustomerName { get; set; }

    public string? PhoneContact { get; set; }

    public string? Email { get; set; }

    public string? Note { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }
    //[NotMapped]
    //public List<string>? Image
    //{
    //    get
    //    {
    //        if (string.IsNullOrEmpty(Image))
    //            return new List<string>();

    //        try
    //        {
    //            return JsonSerializer.Deserialize<List<string>>(Image);
    //        }
    //        catch
    //        {
    //            return new List<string>();
    //        }
    //    }
    //    set
    //    {
    //        if (value == null || !value.Any())
    //        {
    //            Image = null;
    //        }
    //        else
    //        {
    //            Image = JsonSerializer.Serialize(value);
    //        }
    //    }
    //}

    public string? Style { get; set; }

    public string? BuildingCode { get; set; }

    public string? Passkey { get; set; }

    public decimal? Refund { get; set; }

    public string? ShortCode { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual Customer? CustomerCodeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<PaymentHistory> PaymentHistories { get; set; } = new List<PaymentHistory>();

    public virtual ICollection<TrackingHistory> TrackingHistories { get; set; } = new List<TrackingHistory>();
    public virtual ICollection<PaymentResult> PaymentResults { get; set; } = new List<PaymentResult>();


}
