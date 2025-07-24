using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
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
            _context.Payments.Remove(_context.Payments.Find(id));
            return _context.SaveChanges() > 0;
        }

        public List<Payment> GetAllPayments()
        {
            return _context.Payments.ToList();
        }

        public Payment GetPaymentById(int id)
        {
            return _context.Payments.Find(id);
        }

        public bool UpdatePayments(Payment payment)
        {
            _context.Payments.Update(payment);
            return _context.SaveChanges() > 0;
        }
    }
}
