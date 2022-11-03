using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb
{
    public class BookingSystem : IBookingSystem
    {

        public BookingSystem()
        {
            Reservations = new List<IReservation>();
            Reservations.Add(new Reservation("Magnus", 4, DateTime.Parse("2022-11-05"), "19:00", "3"));
            Reservations.Add(new Reservation("Ellen Fiske", 2, DateTime.Parse("2022-11-05"), "20:00", "4"));
        }

        public List<IReservation> Reservations { get; set; }
        public int SeatsAtTable { get; set; }

        public void BookTable(IReservation reservation)
        {
            Reservations.Add(reservation);
        }

        public void CancelReservation()
        {
           
        }

        public void ListBookings()
        {
            
        }

        public void LoadReservations()
        {
            
        }

        public void SaveReservations()
        {
            
        }
    }
}
