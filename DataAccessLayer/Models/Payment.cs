using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal PaymentAmount { get; set; }

    public int MethodId { get; set; }

    public DateTime PaymentDate { get; set; }

    public int OrderId { get; set; }

    public virtual PaymentMethod Method { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
