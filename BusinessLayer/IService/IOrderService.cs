using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IOrderService
    {
        public List<Order> GetAllOrders();
        public Order GetOrderById(int id);
        public bool AddOrder(Order order);
        public bool UpdateOrder(Order order);
        public bool DeleteOrder(int id);
    }
}
