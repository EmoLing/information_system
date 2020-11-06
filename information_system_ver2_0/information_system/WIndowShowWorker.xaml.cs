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
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace information_system
{
    /// <summary>
    /// Логика взаимодействия для WIndowShowWorker.xaml
    /// </summary>
    public partial class WIndowShowWorker : Window
    {
        Repository temp_rep = new Repository();
        int worker_selected;
        public WIndowShowWorker(Repository repository, int worker_selected)
        {
            temp_rep = repository;
            this.worker_selected = worker_selected;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            BoxFirstName.Text = temp_rep.workers[worker_selected].FirstName;
            BoxLastName.Text = temp_rep.workers[worker_selected].LastName;
            BoxAge.Text = temp_rep.workers[worker_selected].Age.ToString();
            BoxDepartment.Text = temp_rep.workers[worker_selected].Department;
            BoxSalary.Text = temp_rep.workers[worker_selected].Salary.ToString();
            BoxPosition.Text = temp_rep.workers[worker_selected].Position;

            string pos_pic = string.Empty;
            switch (temp_rep.workers[worker_selected].Position)
            {
                case "Интерн":
                    {
                        pos_pic = "intern.png";
                        break;
                    }
                case "Рабочий":
                    {
                        pos_pic = "Developer.jpg";
                        break;
                    }
                case "Менеджер":
                    {
                        pos_pic = "itmanager.jpg";
                        break;
                    }
                case "Директор":
                    {
                        pos_pic = "director.jpg";
                        break;
                    }
                default:
                    break;
            }
            string s_uri = @"/information_system;component/pic/Positions/";
            Uri uriImageSource = new Uri(s_uri+pos_pic, UriKind.RelativeOrAbsolute);
            image.Source = new BitmapImage(uriImageSource);
        }
    }
}
