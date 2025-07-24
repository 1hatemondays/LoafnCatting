using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IOrderDetailService
    {
        public List<OrderDetail> GetAllOrderDetails();
        public OrderDetail GetByOrderId(int orderId);
        public bool AddOrderDetail(OrderDetail orderDetail);
        public bool UpdateOrderDetail(OrderDetail orderDetail);
        public bool DeleteOrderDetail(int orderId, int productId);
    }
}
