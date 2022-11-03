using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Labb
{
    internal class Reservation : INotifyPropertyChanged, IReservation
    {
        private string? name;
        private int people;
        private DateTime date;
        private string? table;
        private string? time;

        public Reservation(string name, int people, DateTime date, string time, string table)
        {
            Name = name;
            People = people;
            Date = date;
            Time = time;
            Table = table;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public int People
        {
            get { return people; }
            set
            {
                people = value;
                OnPropertyChanged("People");
            }
        }
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }
        public string Table
        {
            get { return table; }
            set
            {
                table = value;
                OnPropertyChanged("Table");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
