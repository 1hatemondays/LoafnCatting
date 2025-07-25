using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class OrderStatus
    {
        //Add this
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
