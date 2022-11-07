using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Labb
{
    public class BookingSystem : IBookingSystem
    {

        public BookingSystem()
        {
            Reservations = new List<IReservation>();
            //Reservations.Add(new Reservation("Magnus", 4, DateTime.Parse("2022-11-05"), "19:00", "3"));
            //Reservations.Add(new Reservation("Ellen Fiske", 2, DateTime.Parse("2022-11-05"), "20:00", "4"));
        }

        public List<IReservation> Reservations { get; set; }
        private string FileName { get; set; }
        

        public void BookTable(IReservation reservation)
        {

            //if (IsDoubleBooking(reservation))
            //    MessageBox.Show($"{reservation.Table} är redan bokat den valda tiden. Prova med ett annat bord.", "Dubbelbokning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            //else
                Reservations.Add(reservation);
        }

        public void CancelReservation()
        {

        }

        public void ListBookings()
        {

        }

        public bool IsDoubleBooking(IReservation reservation)
        {
            return Reservations.Where(item => item.Date.Equals(reservation.Date)).ToList().Where(item => item.Time.Equals(reservation.Time)).ToList().Any(item => item.Table.Equals(reservation.Table)); 
        }
        public bool IsDoubleBooking(IReservation reservation, int index)
        {
            Reservations.RemoveAt(index);
            bool result = Reservations.Where(item => item.Date.Equals(reservation.Date)).ToList().Where(item => item.Time.Equals(reservation.Time)).ToList().Any(item => item.Table.Equals(reservation.Table)); 
            if (result == false)
                Reservations.Insert(index, reservation);
            return result;
        }

        public async Task LoadReservations()
        {
            if (!File.Exists(FileName))
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Bokningar";
                dlg.DefaultExt = ".json";
                dlg.Filter = "bokning file (.json)|*.json";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    try
                    {

                        string filename = dlg.FileName;
                        FileName = filename;
                        using FileStream openStream = File.OpenRead(filename);
                        List<Reservation>? getReservationList =
                        await JsonSerializer.DeserializeAsync<List<Reservation>>(openStream);
                        Reservations.Clear();
                        Reservations.AddRange(getReservationList);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fel:\n{ex}", "Något gick fel!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                try
                {
                    string filename = FileName;
                    using FileStream openStream = File.OpenRead(filename);
                    List<Reservation>? getReservationList =
                    await JsonSerializer.DeserializeAsync<List<Reservation>>(openStream);
                    Reservations.Clear();
                    Reservations.AddRange(getReservationList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fel:\n{ex}", "Något gick fel!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }



        }

        public async Task SaveReservations()
        {
            if (!File.Exists(FileName))
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Bokningar";
                dlg.DefaultExt = ".json";
                dlg.Filter = "bokning file (.json)|*.json";
                Nullable<bool> result = dlg.ShowDialog();


                if (result == true)
                {
                    try
                    {
                        string filename = dlg.FileName;
                        FileName = filename;
                        using FileStream createStream = File.Create(filename);
                        await JsonSerializer.SerializeAsync(createStream, Reservations);
                        await createStream.DisposeAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fel:\n{ex}", "Något gick fel!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            else
            {
                try
                {

                    string filename = FileName;
                    using FileStream createStream = File.Create(filename);
                    await JsonSerializer.SerializeAsync(createStream, Reservations);
                    await createStream.DisposeAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fel:\n{ex}", "Något gick fel!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

    }
}
