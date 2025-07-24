using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Order> OrderCustomerUsers { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderStaffUsers { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Role Role { get; set; } = null!;
}
