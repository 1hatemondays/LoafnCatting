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
            throw new NotImplementedException();
        }

        public bool DeleteOrderDetail(int orderId, int productId)
        {
            throw new NotImplementedException();
        }

        public List<OrderDetail> GetAllOrderDetails()
        {
            throw new NotImplementedException();
        }

        public OrderDetail GetByOrderId(int orderId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrderDetail(OrderDetail orderDetail)
        {
            throw new NotImplementedException();
        }
    }
}
