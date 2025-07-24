using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
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
            Order order = _context.Orders.FirstOrDefault(o => o.OrderId == id); 
            if(order == null)
            {
                return false;
            }
            _context.Orders.Remove(order);
            return _context.SaveChanges() > 0;
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList();    
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefault(o=>o.OrderId==id);
        }

        public List<Order> GetOrdersByStaffId(int staffUserId)
        {
            return _context.Orders.Where(o => o.StaffUserId == staffUserId).ToList();
        }

        public List<Order> GetOrdersByTableId(int tableId)
        {
            return _context.Orders.Where(o => o.TableId == tableId).ToList();   
        }

        public bool UpdateOrder(Order order)
        {
            Order existingOrder = _context.Orders.FirstOrDefault(o => o.OrderId == order.OrderId);
            if (existingOrder == null)
            {
                return false;
            }
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.StaffUserId = order.StaffUserId;
            existingOrder.TableId = order.TableId;
            existingOrder.TotalPrice= order.TotalPrice;

            return _context.SaveChanges() > 0;
        }
    }
}
