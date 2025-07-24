using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly LoafNcattingDbContext _context;
        public OrderDetailRepository(LoafNcattingDbContext context)
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
            OrderDetail orderDetail = _context.OrderDetails.
                FirstOrDefault(od => od.
                OrderId == orderId && 
                od.ProductId == productId);

            if (orderDetail == null)
            {
                return false;
            }
            _context.OrderDetails.Remove(orderDetail);
            return _context.SaveChanges() > 0;

        }

        public List<OrderDetail> GetAllOrderDetails()
        {
            return _context.OrderDetails.Include(od=>od.Product).ToList();
        }

        public List<OrderDetail> GetByOrderId(int orderId)
        {
            return _context.OrderDetails.Include(od=>od.Product)
                .Where(od => od.OrderId == orderId).ToList();
        }

        public bool UpdateOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            return _context.SaveChanges() > 0;
        }

    }
}
