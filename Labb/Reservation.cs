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
        private string name;
        private string guests;
        private string date;
        private string table;
        private string time;

        public Reservation(string name, string guests, string date, string time, string table)
        {
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

    }
}
