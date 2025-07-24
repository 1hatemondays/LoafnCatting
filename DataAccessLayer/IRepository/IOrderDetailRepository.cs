using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IOrderDetailRepository
    {
        public List<OrderDetail> GetAllOrderDetails();
        public List<OrderDetail> GetByOrderId(int orderId);
        public bool AddOrderDetail(OrderDetail orderDetail);
        public bool UpdateOrderDetail(OrderDetail orderDetail);
        public bool DeleteOrderDetail(int orderId, int productId);
    }
}
