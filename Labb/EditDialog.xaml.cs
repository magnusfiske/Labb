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
            bookingPanel.DataContext = this.bookingList;
            SetInDate();
        }

        public int Index { get; set; }
        public DateTime? Date { get; set; }


        private void SetInDate()
        {
            Reservation tmp = bookingList.SelectedItem as Reservation;
            datePicker.SelectedDate = DateTime.Parse(tmp.Date);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Index = bookingList.SelectedIndex;
            Date = datePicker.SelectedDate;
            OnDateChanged(e);
        }

        public event EventHandler DateChanged;

        private void OnDateChanged(EventArgs e)
        {
            var dateChanged = this.DateChanged;
            dateChanged?.Invoke(this, e);
        }
    }
}
