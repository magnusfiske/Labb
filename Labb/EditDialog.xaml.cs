using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labb
{
    /// <summary>
    /// Interaction logic for EditDialog.xaml
    /// </summary>
    public partial class EditDialog : Window
    {
        private ListView bookingList;


        public EditDialog(ListView bookingList)
        {
            InitializeComponent();

            this.bookingList = bookingList;
            bookingPanelEdit.DataContext = this.bookingList;
            SetInDate();
        }


        public event EventHandler DateChanged;


        public DateTime? NewDate { get; set; }

        public string guestName { get; set; }

        public string Time { get; set; }

        public string Table { get; set; }

        public string Guests { get; set; }


        private void OnDateChanged(EventArgs e)
        {
            var dateChanged = this.DateChanged;
            dateChanged?.Invoke(this, e);
        }

        private void SetInDate()
        {
            Reservation? tmp = bookingList.SelectedItem as Reservation;
            datePickerEdit.SelectedDate = DateTime.Parse(tmp.Date);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public void datePickerEdit_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            NewDate = datePickerEdit.SelectedDate;
            OnDateChanged(e);
        }

        private void tbNameEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            guestName = tbNameEdit.Text;
        }

        public void comboTimeEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i1 = comboTimeEdit.SelectedIndex;
            Time = $"{i1 + 18}:00";
            OnDateChanged(e);
        }

        public void comboGuestsEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i3 = comboGuestsEdit.SelectedIndex;    
            Guests = $"{i3 + 1}";
        }

        private void comboTableEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i2 = comboTableEdit.SelectedIndex;
            Table = $"bord {i2 + 1}";
            OnDateChanged(e);
        }

    }
}
