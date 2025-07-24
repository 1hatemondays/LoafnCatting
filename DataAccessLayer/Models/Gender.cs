using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Gender
{
    public int GenderId { get; set; }

    public string GenderName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Cat> Cats { get; set; } = new List<Cat>();
}
