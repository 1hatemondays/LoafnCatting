using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class RestaurantTable
{
    public int TableId { get; set; }

    public string TableName { get; set; } = null!;

    public int TableStatusId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual TableStatus TableStatus { get; set; } = null!;
}
