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
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public bool AddOrder(Order order)
        {
            return _orderRepository.AddOrder(order);
        }

        public bool DeleteOrder(int id)
        {
            return _orderRepository.DeleteOrder(id);
        }

        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public Order GetOrderById(int id)
        {
            return _orderRepository.GetOrderById(id);
        }

        public List<Order> GetOrdersByStaffId(int staffUserId)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetOrdersByTableId(int tableId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrder(Order order)
        {
            return _orderRepository.UpdateOrder(order);
        }
    }
}
