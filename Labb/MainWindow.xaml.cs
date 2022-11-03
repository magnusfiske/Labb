using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        private int clickCount = 1;

        public MainWindow()
        {
            InitializeComponent();


            MyBookingSystem = new BookingSystem();
            lwBookingList.ItemsSource = MyBookingSystem.Reservations;
            bookingPanel.DataContext = MyBookingSystem;
            SetCombos();
            RefreshContent();
        }

        public BookingSystem MyBookingSystem
        {
            get { return myBookingSystem; }
            set { myBookingSystem = value; }
        }



        public void btnBook_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = datePicker.SelectedDate.Value.Date; //.ToString("ddd d MMM", CultureInfo.CurrentCulture);
            string name = tbName.Text;
            string time = comboTime.Text;
            string table = comboTable.Text;
            int people = Int32.Parse(comboGuests.Text);


            Reservation reservation = new Reservation(name, people, date, time, table);
            MyBookingSystem.BookTable(reservation);
            RefreshContent();
            ClearInputPanel();
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

        private void EnableButton(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate == null || tbName == null || comboTime.SelectedItem == null || comboTable.SelectedItem == null || comboGuests == null)
                return;
            else
            {
                btnBook.IsEnabled = true;
            }

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
            
            
            //if (clickCount % 2 == 1)
            //{
            //    if (MyBookingSystem.Reservations == null || lwBookingList.SelectedIndex == -1)
            //    {
            //        MessageBox.Show("Markera en bokning för att redigera den.", "Fel!", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //    else
            //    {
            //        //var itemToEdit = MyBookingSystem.Reservations.Where(item => item.Id.Equals(lwBookingList.SelectedItem)).Select(item => item.Id);
            //        comboGuests.SelectedItem = MyBookingSystem.Reservations.ElementAt(lwBookingList.SelectedIndex).People;
            //        comboTable.SelectedItem = MyBookingSystem.Reservations.ElementAt(lwBookingList.SelectedIndex).Table;
            //        comboTime.SelectedItem = MyBookingSystem.Reservations.ElementAt(lwBookingList.SelectedIndex).Time;
            //        datePicker.SelectedDate = MyBookingSystem.Reservations.ElementAt(lwBookingList.SelectedIndex).Date;
            //        tbName.Text = MyBookingSystem.Reservations.ElementAt(lwBookingList.SelectedIndex).Name;
                    
            //        clickCount++;
            //        btnEdit.Content = "Spara ändring";
            //    }
            //}
            //else
            //{
            //    //MyBookingSystem.Reservations.ElementAt(lwBookingList.SelectedIndex).People = comboGuests.SelectedItem;
            //}

           
        }
        
        public void ChangeButton(object sender, RoutedEventArgs e)
        {
            
            btnBook.Content = "Spara";
            tbName.Text = "Funkar";
            //btnBook.Click = "btnBook2_Click";
        }

        private void lwBookingList_GotFocus(object sender, RoutedEventArgs e)
        {
            
            ChangeButton(sender, e);   
        }
    }
}
