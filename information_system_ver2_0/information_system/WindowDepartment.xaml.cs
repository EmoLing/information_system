using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для WindowDepartment.xaml
    /// </summary>
    public partial class WindowDepartment : Window
    {
        Repository temp_rep = new Repository();
        TreeViewItem viewItem = new TreeViewItem();
        public WindowDepartment(Repository repository)
        {
            temp_rep = repository;
            InitializeComponent();
        }

        public WindowDepartment(Repository repository, TreeViewItem viewItem)
        {
            temp_rep = repository;
            this.viewItem = viewItem;

            InitializeComponent();
            CheckMainDep.IsChecked = true;
        }

        #region CheckBox

        /// <summary>
        /// CheckBox = True  - есть главный отдел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckMainDep_Checked(object sender, RoutedEventArgs e)
        {

            if (temp_rep.departments.Count != 0)
            {
                ComboDepartment.IsEnabled = true;

                if (viewItem.Header != null && viewItem.Header.ToString() != "Организация")
                {
                    ComboDepartment.Text = viewItem.Header.ToString();
                }
            }
            else

            {
                MessageBox.Show("Departments not found", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                CheckMainDep.IsChecked = false;
            }
            // }
        }

        /// <summary>
        /// CheckBox = False  - нету главного отдела
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckMainDep_Unchecked(object sender, RoutedEventArgs e)
        {
            ComboDepartment.Text = string.Empty;
            ComboDepartment.IsEnabled = false;
        }
        #endregion

        /// <summary>
        /// Обработка нажатия на кнопку Добавления отдела
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButAddDep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int n = temp_rep.departments.Count; //количество отделов


                if ((BoxName.Text != String.Empty) &
                    (BoxDate.Text != String.Empty) & CheckName(n, temp_rep, BoxName.Text)) //проверка на пустоту строк 
                                                                                           //и уникальное название
                {
                    if (TextIsDate(BoxDate.Text))
                    {
                        if (CheckMainDep.IsChecked == false)
                        {
                            if (n == 0)
                            {
                                temp_rep.departments.Add(new Department(0,
                                            BoxName.Text,
                                            Convert.ToDateTime(BoxDate.Text), 0, 0, -1));
                            }
                            else
                            {
                                int[] massID = new int[n];
                                for (int i = 0; i < n; i++)
                                {
                                    massID[i] = temp_rep.departments[i].ID;
                                }
                                temp_rep.departments.Add(new Department(massID.Max() + 1,
                                                                        BoxName.Text,
                                                                        Convert.ToDateTime(BoxDate.Text), 0, 0, -1));
                            }
                            Close();
                        }
                        else
                        {
                            if (ComboDepartment.SelectedItem == null)

                            {
                                MessageBox.Show("Отдел не выбран!","ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
                            }
                            else
                            {
                                //Получение ID основного отдела
                                int ID_dep = 0;
                                int ID_child = 0;
                                int ID_Parent = 0;
                                int ID_in_Mass = 0;
                                for (int i = 0; i < temp_rep.departments.Count; i++)
                                {
                                    if (temp_rep.departments[i].Name == ComboDepartment.SelectedItem.ToString())
                                    {
                                        ID_dep = temp_rep.departments[i].ID;
                                        ID_child = 1 + temp_rep.departments[i].childID;
                                        ID_in_Mass = temp_rep.departments[i].UniqID;
                                        if (temp_rep.departments[i].ParentID < 1)
                                        {
                                            ID_Parent = 1 + temp_rep.departments[i].ParentID;
                                        }
                                        ID_Parent = 0;
                                        break;
                                    }
                                }

                                temp_rep.departments.Add(new Department(ID_dep,
                                                                        BoxName.Text, Convert.ToDateTime(BoxDate.Text),
                                                                        ID_Parent, ID_child, ID_in_Mass));
                                Close();
                            }
                        }
                       
                    }
                    else MessageBox.Show("ДАТА НЕ ВЕРНА \nВВЕДИТЕ ДАТУ В ФОРМАТЕ DD.MM.YYYY", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Не все поля введены!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Проверка на то, что строка является Датой
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static bool TextIsDate(string text)
        {
            var dateFormat = "dd.MM.yyyy";
            var dateFormat2 = "dd,MM,yyyy";
            DateTime scheduleDate;
            if (DateTime.TryParseExact(text, dateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out scheduleDate)
                || DateTime.TryParseExact(text, dateFormat2, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out scheduleDate))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Открытие окна календаря (еще не готово)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<DateTime> date_time_of_create = new List<DateTime>();
            Window1 window1 = new Window1(date_time_of_create);

            window1.Owner = this;
            this.IsEnabled = false;
            if (window1.ShowDialog() == true)
            {
                window1.Show();
                window1.Activate();
            }
            this.IsEnabled = true;
            if (date_time_of_create.Count > 0)
            {
                BoxDate.Text = date_time_of_create[0].Date.ToShortDateString();
            }

        }

        /// <summary>
        /// Проверка на совпадение названий отдела
        /// </summary>
        /// <param name="n"></param>
        /// <param name="temp_rep"></param>
        /// <param name="BoxName"></param>
        /// <returns></returns>
        static bool CheckName(int n, Repository temp_rep, string BoxName)
        {
            if (n > 0)
            {
                string[] massName = new string[n];

                for (int i = 0; i < n; i++)
                {
                    massName[i] = temp_rep.departments[i].Name;
                }

                for (int i = 0; i < n; i++)
                {
                    if (massName[i] == BoxName)
                    {
                        MessageBox.Show("Такое название уже есть!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
            }
            return true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            for (int i = 0; i < temp_rep.departments.Count; i++)
                ComboDepartment.Items.Add(newItem: temp_rep.departments[i].Name);

            if (viewItem.Header == null || viewItem.Header.ToString() == "Организация")
            {
                CheckMainDep.IsChecked = false;
            }
        }
    }
}
