using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly LoafNcattingDbContext _context;
        public ReservationRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }
        public bool AddReservation(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public bool DeleteReservation(int id)
        {
            throw new NotImplementedException();
        }

        public List<Reservation> GetAllReservation()
        {
            throw new NotImplementedException();
        }

        public Reservation GetReservationById(int id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateReservation(Reservation reservation)
        {
            throw new NotImplementedException();
        }
    }
}
