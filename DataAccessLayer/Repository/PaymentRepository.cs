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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly LoafNcattingDbContext _context;
        public PaymentRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }
        public bool AddPayments(Payment payment)
        {
            _context.Payments.Add(payment);
            return _context.SaveChanges() > 0;
        }

        public bool DeletePayments(int id)
        {
            Payment payment = _context.Payments.FirstOrDefault(p=>p.PaymentId==id);
            if (payment == null)
            {
                return false;
            }
            _context.Payments.Remove(payment);
            return _context.SaveChanges() > 0;
        }

        public List<Payment> GetAllPayments()
        {
            return _context.Payments.Include(p=>p.Method).ToList();
        }

        public Payment GetPaymentById(int id)
        {
            return _context.Payments.Include(p => p.Method).FirstOrDefault(p=>p.PaymentId==id);
        }

        public List<Payment> GetPaymentsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .ToList();
        }

        public List<Payment> GetPaymentsByMethodId(int methodId)
        {
            return _context.Payments
                .Where(p => p.MethodId == methodId)
                .ToList();
        }

        public List<Payment> GetPaymentsByOrderId(int orderId)
        {
            return _context.Payments
                .Where(p => p.OrderId == orderId)
                .ToList();  
        }

        public bool UpdatePayments(Payment payment)
        {
            Payment existingPayment = _context.Payments.FirstOrDefault(p => p.PaymentId == payment.PaymentId);
            if (existingPayment == null)
            {
                return false;
            }
            existingPayment.PaymentDate = payment.PaymentDate;
            existingPayment.PaymentAmount = payment.PaymentAmount;
            existingPayment.MethodId = payment.MethodId;
            existingPayment.OrderId = payment.OrderId;
            return _context.SaveChanges() > 0;
        }
    }
}
