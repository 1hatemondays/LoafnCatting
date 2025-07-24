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

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;
}
