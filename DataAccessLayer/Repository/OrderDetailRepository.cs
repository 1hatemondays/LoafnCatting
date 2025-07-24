using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly LoafNcattingDbContext _context;
        public OrderDetailRepository( LoafNcattingDbContext context)
        {
            _context = context;
        }
        public bool AddOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteOrderDetail(int orderId, int productId)
        {
            _context.OrderDetails.Remove(_context.OrderDetails.FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId));   
            return _context.SaveChanges() > 0;

        }

        public List<OrderDetail> GetAllOrderDetails()
        {
            return _context.OrderDetails.ToList();
        }

        public OrderDetail GetByOrderId(int orderId)
        {
            return _context.OrderDetails.FirstOrDefault(od => od.OrderId == orderId);
        }

        public bool UpdateOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            return _context.SaveChanges() > 0;
        }
    }
}
