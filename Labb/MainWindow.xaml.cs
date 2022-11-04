using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Labb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BookingSystem myBookingSystem;

        public MainWindow()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "ddd d MMM";
            Thread.CurrentThread.CurrentCulture = ci;

            InitializeComponent();

            MyBookingSystem = new BookingSystem();
            lwBookingList.ItemsSource = MyBookingSystem.Reservations;
            bookingPanel.DataContext = MyBookingSystem;
            SetCombos();
            RefreshContent();
        }

        //public string FileName { get; set; }

        public List<IReservation> SameDate { get; set; }
        public List<IReservation> SameTime { get; set; }

        public BookingSystem MyBookingSystem
        {
            get { return myBookingSystem; }
            set { myBookingSystem = value; }
        }



        public async void btnBook_Click(object sender, RoutedEventArgs e)
        {
            string date = datePicker.SelectedDate.Value.ToString("ddd d MMM", CultureInfo.CurrentCulture);
            string name = tbName.Text;
            string time = comboTime.Text;
            string table = comboTable.Text;
            int people = Int32.Parse(comboGuests.Text);

            try
            {
                Reservation reservation = new Reservation(name, people, date, time, table);
                MyBookingSystem.BookTable(reservation);
                await myBookingSystem.SaveReservations();
                RefreshContent();
                ClearInputPanel();
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Felaktig inmatning, försök igen\n{ex}", "Inmatningsfel!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel:\n{ex}", "Okänt fel!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MyBookingSystem.Reservations == null || lwBookingList.SelectedIndex == -1)
                MessageBox.Show("Markera en bokning för att ta bort den.","Fel!", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                try
                {
                    myBookingSystem.Reservations.RemoveAt(lwBookingList.SelectedIndex);
                    RefreshContent();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RefreshContent()
        {
            lwBookingList.ItemsSource = null;
            lwBookingList.ItemsSource = myBookingSystem.Reservations;
        }

        private void EnableBookButton(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate == null || tbName == null || comboTime.SelectedItem == null || comboTable.SelectedItem == null || comboGuests == null)
                return;
            else
            {
                btnBook.IsEnabled = true;
            }

        }

        private void EnableEditButton()
        {
            btnEdit.IsEnabled = true;
            btnCancel.IsEnabled = true;
        }

        private void SetCombos()
        {
            for (int i = 18; i < 24; i++)
            {
                comboTime.Items.Add($"{i}:00");
            }
            for (int i = 1; i < 8; i++)
            {
                comboTable.Items.Add($"bord {i}");
            }
        }

        private void ClearInputPanel()
        { 
            comboTime.SelectedIndex = -1; 
            comboTable.SelectedIndex = -1;
            datePicker.SelectedDate = DateTime.Now;
            tbName.Clear();
            comboGuests.SelectedIndex = -1;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new EditDialog(lwBookingList) { Owner = this };
            dlg.DateChanged += dlg_DateChanged;
            dlg.ShowDialog();
        }

        private void lwBookingList_GotFocus(object sender, RoutedEventArgs e)
        {
            EnableEditButton();   
        }

        public async Task WritingBookingDataToFile()
        {
            //if (!File.Exists(FileName))
            //{
            //    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //    dlg.FileName = "Bokningar";
            //    dlg.DefaultExt = ".json";
            //    dlg.Filter = "bokning file (.json)|*.json";
            //    Nullable<bool> result = dlg.ShowDialog();
            //    if (result == true)
            //    {
            //        string filename = dlg.FileName;
            //        FileName = filename;
            //        using FileStream createStream = File.Create(filename);
            //        await JsonSerializer.SerializeAsync(createStream, myBookingSystem.Reservations);
            //        await createStream.DisposeAsync();
            //    }
            //}
            //else
            //{
            //    string filename = FileName;
            //    using FileStream createStream = File.Create(filename);
            //    await JsonSerializer.SerializeAsync(createStream, myBookingSystem.Reservations);
            //    await createStream.DisposeAsync();
            //}
        }

        public async Task LoadBookingDataFromFile()
        {
            //if (!File.Exists(FileName))
            //{
            //    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //    dlg.FileName = "Bokningar";
            //    dlg.DefaultExt = ".json";
            //    dlg.Filter = "bokning file (.json)|*.json";
            //    Nullable<bool> result = dlg.ShowDialog();
            //    try
            //    {
            //        if (result == true)
            //        {
            //            string filename = dlg.FileName;
            //            FileName = filename;
            //            using FileStream openStream = File.OpenRead(filename);
            //            List<Reservation>? getReservationList =
            //            await JsonSerializer.DeserializeAsync<List<Reservation>>(openStream);
            //            myBookingSystem.Reservations.Clear();
            //            myBookingSystem.Reservations.AddRange(getReservationList);
            //        }
            //        else
            //        {
            //            string filename = FileName;
            //            using FileStream openStream = File.OpenRead(filename);
            //            List<Reservation>? getReservationList =
            //            await JsonSerializer.DeserializeAsync<List<Reservation>>(openStream);
            //            myBookingSystem.Reservations.Clear();
            //            myBookingSystem.Reservations.AddRange(getReservationList);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Fel:\n{ex}", "Något gick fel!", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            RefreshContent();

        }

        private void dlg_DateChanged(object sender, EventArgs e)
        {
            var dlg = (EditDialog)sender;

            string newDate = dlg.Date.Value.ToString("ddd d MMM");

            myBookingSystem.Reservations.ElementAt(dlg.Index).Date = newDate;
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            await myBookingSystem.LoadReservations();
            RefreshContent();
        }
    }
}
