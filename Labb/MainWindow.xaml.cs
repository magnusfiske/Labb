using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
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
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        private BookingSystem myBookingSystem;

        private CollectionView view;

        private GridViewColumnHeader listViewSortCol = null;

        private SortAdorner listViewSortAdorner = null;

        public MainWindow()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd MMM ddd";
            Thread.CurrentThread.CurrentCulture = ci;

            InitializeComponent();

            Filter = true;
            MyBookingSystem = new BookingSystem();
            lvBookingList.ItemsSource = MyBookingSystem.Reservations;
            //bookingPanel.DataContext = MyBookingSystem;
            SetCombos();
            RefreshContent();
        }

        private bool Filter { get; set; }

        public BookingSystem MyBookingSystem
        {
            get { return myBookingSystem; }
            set { myBookingSystem = value; }
        }

        private CollectionView View
        {
            get { return view; }
            set { view = value; }
        }

        private async void btnBook_Click(object sender, RoutedEventArgs e)
        {
            string date = datePicker.SelectedDate.Value.ToString("dd MMM ddd", CultureInfo.CurrentCulture);
            string name = tbName.Text;
            string time = comboTime.Text;
            string table = comboTable.Text;
            string guests = comboGuests.Text;

            try
            {
                Reservation reservation = new Reservation(name, guests, date, time, table);

                if (MyBookingSystem.IsDoubleBooking(reservation))
                    MessageBox.Show($"{reservation.Table} är redan bokat den valda tiden. Prova med ett annat bord.", "Dubbelbokning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (MyBookingSystem.HasFive(reservation))
                    MessageBox.Show($"Det finns redan 5 bord bokade kl {reservation.Time}. Prova med en annan tid eller be gästen komma obokad.", "För många bokningar", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                {
                    MyBookingSystem.BookTable(reservation);
                    await MyBookingSystem.SaveReservations();
                    ClearInputPanel();
                    //Filter = false;
                    //btnShowAll.Content = "Filtrera";
                    RefreshContent();
                }
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MyBookingSystem.Reservations == null || lvBookingList.SelectedIndex == -1)
                MessageBox.Show("Markera en bokning för att ta bort den.", "Fel!", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                try
                {
                    Reservation bookingToCancel = lvBookingList.SelectedItem as Reservation;
                    MyBookingSystem.CancelReservation(bookingToCancel);
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
            lvBookingList.ItemsSource = null;
            lvBookingList.ItemsSource = MyBookingSystem.Reservations;
            

            if (Filter == true)
            {
                View = (CollectionView)CollectionViewSource.GetDefaultView(lvBookingList.ItemsSource);
                View.Filter = BookingFilter;
            }
            else
            {
                View.Filter = null;
            }
        }

        private void EnableBookButton(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate == null || string.IsNullOrEmpty(tbName.Text) || comboTime.SelectedItem == null || comboTable.SelectedItem == null || comboGuests.SelectedItem == null)
                btnBook.IsEnabled = false;
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
            tbName.Clear();
            comboGuests.SelectedIndex = -1;
        }

       
        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MyBookingSystem.Reservations == null || lvBookingList.SelectedIndex == -1)
                MessageBox.Show("Markera en bokning för att redigera den.", "Fel!", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                try
                {
                    var dlg = new EditDialog(lvBookingList) { Owner = this };
                    dlg.DateChanged += dlg_DateChanged;
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        dlg.DateChanged -= dlg_DateChanged;
                        string newDate = dlg.NewDate.Value.ToString("dd MMM ddd");
                        var bookingToUpdate = MyBookingSystem.Reservations.FirstOrDefault(item => item.ReservationId.Equals(dlg.ReservationId));
                        bookingToUpdate.Date = newDate;
                        await MyBookingSystem.SaveReservations();
                    }
                    else
                    {
                        dlg.DateChanged -= dlg_DateChanged;
                       
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            RefreshContent();
        }

        private void dlg_DateChanged(object sender, EventArgs e)
        {
            try
            {
                var dlg = (EditDialog)sender;
                string newDate = dlg.NewDate.Value.ToString("dd MMM ddd");

                Reservation bookingToUpdate = MyBookingSystem.Reservations.FirstOrDefault(item => item.ReservationId.Equals(dlg.ReservationId)) as Reservation;
                bookingToUpdate.Date = newDate;
                bookingToUpdate.Time = dlg.Time;
                bookingToUpdate.Table = dlg.Table;

                if (MyBookingSystem.IsDoubleBooking(bookingToUpdate))
                {
                    dlg.btnOk.IsEnabled = false;
                    MessageBox.Show($"{dlg.Table} är redan bokat den valda tiden. Prova med ett annat bord.", "Dubbelbokning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (MyBookingSystem.HasFive(bookingToUpdate))
                {
                    dlg.btnOk.IsEnabled = false;
                    MessageBox.Show($"Det finns redan 5 bord bokade kl {dlg.Time}. Prova med en annan tid eller be gästen komma obokad.", "För många bokningar", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    dlg.btnOk.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Något gick fel!");
            }
        }
        private void lvBookingList_GotFocus(object sender, RoutedEventArgs e)
        {
            EnableEditButton();
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            await MyBookingSystem.LoadReservations();
            RefreshContent();
        }

        private void comboTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableBookButton(sender, e);
            RefreshContent();
        }

        private bool BookingFilter(object item)
        {
            if (datePicker.SelectedDate == null && comboTime.SelectedItem == null && comboTable.SelectedItem == null)
                return true;
            else if (comboTime.SelectedItem == null && comboTable.SelectedItem == null)
                return (item as Reservation).Date.Equals(datePicker.SelectedDate.Value.ToString("dd MMM ddd"));
            else
                return (item as Reservation).Date.Equals(datePicker.SelectedDate.Value.ToString("dd MMM ddd"))
                    && (item as Reservation).Time.Equals(comboTime.SelectedItem);
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableBookButton(sender, e);

            RefreshContent();
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            if (Filter == true)
            {
                Filter = false;
                btnShowAll.Content = "Filtrera";
                RefreshContent();
            }
            else
            {
                Filter = true;
                btnShowAll.Content = "Visa alla";
                RefreshContent();
            }
        }

        //Den här metoden har jag lånat från wpf-tutorial.com. Till skillnad från SortAdorner har jag däremot bråkat
        //tillräckligt med den för att känna att jag har rätt bra koll på hur den funkar. Men, den ska ses som ett citat.
        private void lvBookingListColumnHeader_Click(object sender, RoutedEventArgs e) 
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvBookingList.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == ListSortDirection.Ascending)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);

            lvBookingList.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }
}
