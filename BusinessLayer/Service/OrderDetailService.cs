using BusinessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository; 
        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public bool AddOrderDetail(OrderDetail orderDetail)
        {
            return _orderDetailRepository.AddOrderDetail(orderDetail);
        }

        public bool DeleteOrderDetail(int orderId, int productId)
        {
            return _orderDetailRepository.DeleteOrderDetail(orderId, productId);
        }

        public List<OrderDetail> GetAllOrderDetails()
        {
            return _orderDetailRepository.GetAllOrderDetails();
        }

        public OrderDetail GetByOrderId(int orderId)
        {
            return _orderDetailRepository.GetByOrderId(orderId);
        }

        public bool UpdateOrderDetail(OrderDetail orderDetail)
        {
            return _orderDetailRepository.UpdateOrderDetail(orderDetail);
        }
    }
}
