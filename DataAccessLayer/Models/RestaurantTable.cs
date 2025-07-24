using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class RestaurantTable
{
    public int TableId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
