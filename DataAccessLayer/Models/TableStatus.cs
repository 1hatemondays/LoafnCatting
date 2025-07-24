using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class TableStatus
{
    public int TableStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<RestaurantTable> RestaurantTables { get; set; } = new List<RestaurantTable>();
}
