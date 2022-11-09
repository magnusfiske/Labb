using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Labb
{
    public class Reservation :  IReservation
    {
        private Guid reservationId;
        private string name;
        private string guests;
        private string date;
        private string table;
        private string time;

        public Reservation(string name, string guests, string date, string time, string table)
        {
            ReservationId = Guid.NewGuid();
            Name = name;
            Guests = guests;
            Date = date;
            Time = time;
            Table = table;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Guests
        {
            get { return guests; }
            set { guests = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        public Guid ReservationId
        {
            get { return reservationId; }
            set { reservationId = value; }
        }
    }
}
