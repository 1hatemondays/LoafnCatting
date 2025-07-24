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
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public bool AddReservation(Reservation reservation)
        {
            return _reservationRepository.AddReservation(reservation);
        }

        public bool DeleteReservation(int id)
        {
            return _reservationRepository.DeleteReservation(id);
        }

        public List<Reservation> GetAllReservation()
        {
            return _reservationRepository.GetAllReservation();
        }

        public Reservation GetReservationById(int id)
        {
            return _reservationRepository.GetReservationById(id);   
        }

        public bool UpdateReservation(Reservation reservation)
        {
            return _reservationRepository.UpdateReservation(reservation);   
        }
    }
}
