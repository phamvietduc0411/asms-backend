using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Contact
{
    public int ContactId { get; set; }

    public string? CustomerCode { get; set; }

    public string? EmployeeCode { get; set; }

    public string? OrderCode { get; set; }

    public string? Name { get; set; }

    public string? PhoneContact { get; set; }

    public string? Email { get; set; }

    public string Message { get; set; } = null!;

    public virtual Customer? CustomerCodeNavigation { get; set; }

    public virtual Order? OrderCodeNavigation { get; set; }
}
