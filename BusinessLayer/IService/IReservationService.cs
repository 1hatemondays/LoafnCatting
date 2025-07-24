using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IReservationService
    {
        public List<Reservation> GetAllReservation();
        public Reservation GetReservationById(int id);
        public bool AddReservation(Reservation reservation);
        public bool UpdateReservation(Reservation reservation);
        public bool DeleteReservation(int id);
        public List<Reservation> GetReservationsByDate(DateTime date);
        public List<Reservation> GetReservationsByStatusId(int statusId);
    }
}
