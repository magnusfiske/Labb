using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        }

        public List<IReservation> Reservations { get; set; }
        private string? FileName { get; set; }
        

        public void BookTable(IReservation reservation)
        {
                Reservations.Add(reservation);
        }

        public async void CancelReservation(Reservation reservation)
        {
            try
            {
                Reservations.Remove(reservation);
                await SaveReservations();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        public bool IsDoubleBooking(Reservation reservation)
        { 
                
            return Reservations.Where(item => !item.ReservationId.Equals(reservation.ReservationId))
                .Where(item => item.Date.Equals(reservation.Date)).ToList()
                .Where(item => item.Time.Equals(reservation.Time)).ToList()
                .Any(item => item.Table.Equals(reservation.Table));
        }

        

        public bool HasFive(Reservation reservation)
        {
            var number = Reservations.Where(item => item.Date.Equals(reservation.Date)).ToList()
                    .Where(item => item.Time.Equals(reservation.Time)).ToList();

            return (Reservations.Any(item => item.ReservationId.Equals(reservation.ReservationId))) ? number.Count >= 6 : number.Count >= 5;
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
                        FileName = null;
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

        public void SortReservations(string sortBy, bool newDir)
        {
            switch (newDir)
            {
                case true:
                    switch (sortBy)
                    {
                        case "Date":
                            Reservations = Reservations.OrderBy(item => item.Date).ToList();
                            break;
                        case "Time":
                            Reservations = Reservations.OrderBy(item => item.Time).ToList();
                            break;
                        case "Name":
                            Reservations = Reservations.OrderBy(item => item.Name).ToList();
                            break;
                        case "Guests":
                            Reservations = Reservations.OrderBy(item => item.Guests).ToList();
                            break;
                        case "Table":
                            Reservations = Reservations.OrderBy(item => item.Table).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
                case false:
                    switch (sortBy)
                    {
                        case "Date":
                            Reservations = Reservations.OrderByDescending(item => item.Date).ToList();
                            break;
                        case "Time":
                            Reservations = Reservations.OrderByDescending(item => item.Time).ToList();
                            break;
                        case "Name":
                            Reservations = Reservations.OrderByDescending(item => item.Name).ToList();
                            break;
                        case "Guests":
                            Reservations = Reservations.OrderByDescending(item => item.Guests).ToList();
                            break;
                        case "Table":
                            Reservations = Reservations.OrderByDescending(item => item.Table).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
