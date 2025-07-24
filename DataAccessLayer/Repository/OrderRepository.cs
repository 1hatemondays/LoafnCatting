using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LoafNcattingDbContext _context;
        public OrderRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }

        public bool AddOrder(Order order)
        {
            _context.Orders.Add(order);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteOrder(int id)
        {
            _context.Orders.Remove(_context.Orders.Find(id));
            return _context.SaveChanges() > 0;
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList();    
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders.Find(id);
        }

        public bool UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            return _context.SaveChanges() > 0;
        }
    }
}
