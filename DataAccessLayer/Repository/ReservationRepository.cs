using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
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
            Reservation reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                return false;
            }
            _context.Reservations.Remove(reservation);
            return _context.SaveChanges() > 0;
        }

        public List<Reservation> GetAllReservation()
        {
            return _context.Reservations.ToList();  
        }

        public Reservation GetReservationById(int id)
        {

            return _context.Reservations.FirstOrDefault(r => r.ReservationId == id);
        }

        public bool UpdateReservation(Reservation reservation)
        {
            Reservation reservationUpdate = _context.Reservations.FirstOrDefault(r => r.ReservationId == reservation.ReservationId);
            if (reservationUpdate == null)
            {
                return false;
            }
            
            // Update reservation properties
            reservationUpdate.Date = reservation.Date;
            reservationUpdate.Time = reservation.Time;
            reservationUpdate.StatusId = reservation.StatusId;
            reservationUpdate.UserId = reservation.UserId;
            reservationUpdate.TableId = reservation.TableId;
            
            return _context.SaveChanges() > 0;
        }
    }
}
