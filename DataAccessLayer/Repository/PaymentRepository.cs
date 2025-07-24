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
            throw new NotImplementedException();
        }

        public bool DeletePayments(int id)
        {
            throw new NotImplementedException();
        }

        public List<Payment> GetAllPayments()
        {
            throw new NotImplementedException();
        }

        public Payment GetPaymentById(int id)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePayments(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}
