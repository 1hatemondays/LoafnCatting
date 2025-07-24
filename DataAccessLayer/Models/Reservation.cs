using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public int StatusId { get; set; }

    public int UserId { get; set; }

    public int TableId { get; set; }

    public virtual ReservationStatus Status { get; set; } = null!;

    public virtual RestaurantTable Table { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
