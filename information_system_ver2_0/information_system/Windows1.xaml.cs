using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace information_system
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<DateTime> TempDateTime;
        public Window1(List<DateTime> date_time_of_create)
        {
            TempDateTime = date_time_of_create;
            InitializeComponent();
        }

        /// <summary>
        /// Добавление даты в строку в WindowDepartment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TempDateTime.Add((DateTime)NameCalendar.SelectedDate);
            this.Close();
        }
    }
}
