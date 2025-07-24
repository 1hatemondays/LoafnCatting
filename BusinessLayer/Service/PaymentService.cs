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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public bool AddPayments(Payment payment)
        {
            return _paymentRepository.AddPayments(payment); 
        }

        public bool DeletePayments(int id)
        {
            return _paymentRepository.DeletePayments(id);
        }

        public List<Payment> GetAllPayments()
        {
            return _paymentRepository.GetAllPayments();
        }

        public Payment GetPaymentById(int id)
        {
            return _paymentRepository.GetPaymentById(id);
        }

        public List<Payment> GetPaymentsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _paymentRepository.GetPaymentsByDateRange(startDate, endDate);   
        }

        public List<Payment> GetPaymentsByMethodId(int methodId)
        {
            return _paymentRepository.GetPaymentsByMethodId(methodId);  
        }

        public List<Payment> GetPaymentsByOrderId(int orderId)
        {
            return _paymentRepository.GetPaymentsByOrderId(orderId);
        }

        public bool UpdatePayments(Payment payment)
        {
            return _paymentRepository.UpdatePayments(payment);  
        }
    }
}
