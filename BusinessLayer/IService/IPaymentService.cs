using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IPaymentService
    {
        public List<Payment> GetAllPayments();
        public Payment GetPaymentById(int id);
        public bool AddPayments(Payment payment);
        public bool UpdatePayments(Payment payment);
        public bool DeletePayments(int id);
        public List<Payment> GetPaymentsByOrderId(int orderId);
        public List<Payment> GetPaymentsByMethodId(int methodId);
        public List<Payment> GetPaymentsByDateRange(DateTime startDate, DateTime endDate);
    }
}
