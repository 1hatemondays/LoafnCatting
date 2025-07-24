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
            _context.Reservations.Add(reservation);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteReservation(int id)
        {
            _context.Reservations.Remove(_context.Reservations.Find(id));   
            return _context.SaveChanges() > 0;
        }

        public List<Reservation> GetAllReservation()
        {
            return _context.Reservations.ToList();  
        }

        public Reservation GetReservationById(int id)
        {
            return _context.Reservations.Find(id);

        public bool UpdateReservation(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return _context.SaveChanges() > 0;
        }
    }
}
