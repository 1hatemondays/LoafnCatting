using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class CatStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Cat> Cats { get; set; } = new List<Cat>();
}
