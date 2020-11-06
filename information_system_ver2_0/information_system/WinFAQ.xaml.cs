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
using System.IO;
namespace information_system
{
    /// <summary>
    /// Логика взаимодействия для WinFAQ.xaml
    /// </summary>
    public partial class WinFAQ : Window
    {
        public WinFAQ()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            string path = "readme.txt";
            using (StreamReader streamReader = new StreamReader(path))
            {
                readbeBlock.Text = streamReader.ReadToEnd();
            }
        }
    }
}
