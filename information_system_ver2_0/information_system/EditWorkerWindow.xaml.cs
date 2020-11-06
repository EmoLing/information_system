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
    /// Логика взаимодействия для EditWorkerWindow.xaml
    /// </summary>
    public partial class EditWorkerWindow : Window
    {
        Repository temp_rep = new Repository();
        int worker_selected;
        public EditWorkerWindow(Repository repository, int worker_selected)
        {
            temp_rep = repository;
            this.worker_selected = worker_selected;
            InitializeComponent();
        }

        /// <summary>
        /// Загрузка данных выбранного сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            BoxFirstName.Text = temp_rep.workers[worker_selected].FirstName;
            BoxLastName.Text = temp_rep.workers[worker_selected].LastName;
            BoxAge.Text = temp_rep.workers[worker_selected].Age.ToString();
            ComboDepartment.Text = temp_rep.workers[worker_selected].Department;
            ComboPosition.Text = temp_rep.workers[worker_selected].Position;
            BoxSalary.Text = temp_rep.workers[worker_selected].Salary.ToString();
            if (temp_rep.workers[worker_selected].Position == "Рабочий")
                BoxHours.Text = (temp_rep.workers[worker_selected].Salary / 12).ToString();
        }

        #region CheckBoxes Check/Unchecked

        #region FirstName
        private void CheckFirstName_Checked(object sender, RoutedEventArgs e)
        {
            BoxFirstName.IsEnabled = true;
        }

        private void CheckFirstName_Unchecked(object sender, RoutedEventArgs e)
        {
            BoxFirstName.IsEnabled = false;
        }
        #endregion

        #region LastName
        private void CheckLastName_Checked(object sender, RoutedEventArgs e)
        {
            BoxLastName.IsEnabled = true;
        }

        private void CheckLastName_Unchecked(object sender, RoutedEventArgs e)
        {
            BoxLastName.IsEnabled = false;
        }
        #endregion

        #region Age
        private void CheckAge_Checked(object sender, RoutedEventArgs e)
        {
            BoxAge.IsEnabled = true;
        }

        private void CheckAge_Unchecked(object sender, RoutedEventArgs e)
        {
            BoxAge.IsEnabled = false;
        }
        #endregion

        #region Department
        private void CheckDepartment_Checked(object sender, RoutedEventArgs e)
        {
            ComboDepartment.IsEnabled = true;
        }

        private void CheckDepartment_Unchecked(object sender, RoutedEventArgs e)
        {
            ComboDepartment.IsEnabled = false;
        }
        #endregion

        #region Position
        private void CheckPosition_Checked(object sender, RoutedEventArgs e)
        {
            ComboPosition.IsEnabled = true;
        }

        private void CheckPosition_Unchecked(object sender, RoutedEventArgs e)
        {
            ComboPosition.IsEnabled = false;
        }
        #endregion

        #endregion

        private void ButEditWorker_Click(object sender, RoutedEventArgs e)
        {
            int variant = 0;
            var current_worker = temp_rep.workers[worker_selected];
            //  int IdWorker = (temp_rep.workers.Count > 0) ? temp_rep.workers[temp_rep.workers.Count - 1].ID + 1 : 0;
            if (int.Parse(BoxAge.Text) > 255)
            {
                MessageBox.Show("СЛИШКОМ БОЛЬШОЙ ВОЗРАСТ", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                if (ComboPosition.Text == "Интерн")
                {
                    current_worker = temp_rep.workers[worker_selected] as Intern;
                    variant = 0;
                }

                else if (ComboPosition.Text == "Рабочий")
                    if (BoxSalary.Text == string.Empty)
                        MessageBox.Show("Введите количество часов", "WARNING", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        current_worker = temp_rep.workers[worker_selected] as MainWorker;
                        variant = 1;
                    }
                else if (ComboPosition.Text == "Директор")
                {
                    current_worker = temp_rep.workers[worker_selected] as Director;
                    variant = 2;
                }
                else if (ComboPosition.Text == "Менеджер")
                {
                    current_worker = temp_rep.workers[worker_selected] as MainManager;
                    variant = 3;
                }

                if (current_worker == null)
                {
                    if (variant == 0)
                    {
                        current_worker = new Intern(BoxFirstName.Text, BoxLastName.Text, byte.Parse(BoxAge.Text), ComboDepartment.Text, null, 0);
                    }
                    else if (variant == 1)
                    {
                        current_worker = new MainWorker(BoxFirstName.Text, BoxLastName.Text, byte.Parse(BoxAge.Text), ComboDepartment.Text, null, int.Parse(BoxHours.Text), 0);
                        current_worker.Salary = (current_worker as MainWorker).Salary;
                    }
                    else if (variant == 2)
                    {
                        current_worker = new Director(BoxFirstName.Text, BoxLastName.Text, byte.Parse(BoxAge.Text), ComboDepartment.Text, null, 0);
                    }
                    else if (variant == 3)
                    {
                        current_worker = new MainManager(BoxFirstName.Text, BoxLastName.Text, byte.Parse(BoxAge.Text), ComboDepartment.Text, null, 0);
                    }
                }
                //Проверка на поля
                if (CheckFirstName.IsChecked == true)
                {
                    current_worker.FirstName = BoxFirstName.Text;
                }
                if (CheckLastName.IsChecked == true)
                {
                    current_worker.LastName = BoxLastName.Text;
                }
                if (CheckAge.IsChecked == true)
                {
                    current_worker.Age = byte.Parse(BoxAge.Text);
                }
                if (CheckDepartment.IsChecked == true)
                {
                    current_worker.Department = ComboDepartment.Text;
                }
                if (CheckPosition.IsChecked == true)
                {
                    current_worker.Position = ComboPosition.Text;
                }
                current_worker.ID--;
                temp_rep.workers[worker_selected].LastId();
                temp_rep.workers[worker_selected] = current_worker;
                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        #region ComboBox

        /// <summary>
        /// Интерн
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboIntern_Selected(object sender, RoutedEventArgs e)
        {
            ComboDepartment.IsEnabled = true;
            BoxSalary.Text = string.Empty;
            BoxSalary.Text = "500";
            BoxHours.IsEnabled = false;
            SaveHours.IsEnabled = false;
        }

        /// <summary>
        /// Рабочий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboMainWorker_Selected(object sender, RoutedEventArgs e)
        {
            ComboDepartment.IsEnabled = true;
            BoxSalary.Text = string.Empty;
            BoxHours.IsEnabled = true;
            SaveHours.IsEnabled = true;
        }

        /// <summary>
        /// Директор
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDirector_Selected(object sender, RoutedEventArgs e)
        {
            ComboDepartment.IsEnabled = false;
            BoxSalary.Text = string.Empty;

            //Проверка на то, есть ли сотрудники
            if (temp_rep.workers.Count > 0)
            {
                bool true_false = true;

                //Поиск других директоров
                for (int i = 0; i < temp_rep.workers.Count; i++)
                {
                    if (temp_rep.workers[i].Position == "Директор")
                        true_false = false;
                }

                //Если вернет true, то будет создаваться директор
                if (true_false)
                {
                    ComboPosition.Text = string.Empty;
                    MessageBox.Show("Директор уже есть", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Создайте вначале рабочего/интерна", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            BoxHours.IsEnabled = false;
            SaveHours.IsEnabled = false;
        }

        /// <summary>
        /// Менеджер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboMainManager_Selected(object sender, RoutedEventArgs e)
        {
            BoxSalary.Text = string.Empty;
            ComboDepartment.IsEnabled = true;
            BoxHours.IsEnabled = false;
            SaveHours.IsEnabled = false;
        }

        #endregion

        /// <summary>
        /// Инициализация отделов при загрузке окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDepartment_Initialized(object sender, EventArgs e)
        {
            for (int i = 0; i < temp_rep.departments.Count; i++)
                ComboDepartment.Items.Add(newItem: temp_rep.departments[i].Name);
        }

        /// <summary>
        /// Сохранение количества часов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveHours_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BoxHours.Text != string.Empty)
                    BoxSalary.Text = (12 * int.Parse(BoxHours.Text)).ToString();
                else MessageBox.Show("Поле количества часов пустое!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ввод только цифр для Возраста
        /// </summary>
        private void BoxAge_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Ввод только цифр для часов работы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BoxHours_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

    }
}
