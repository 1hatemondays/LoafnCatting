using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Order
{
    //Fix
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public int StaffUserId { get; set; }

    public int TableId { get; set; }
    public int OrderStatusId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User StaffUser { get; set; } = null!;

    public virtual RestaurantTable Table { get; set; } = null!;
    public virtual OrderStatus OrderStatus { get; set; } = null!;
}
