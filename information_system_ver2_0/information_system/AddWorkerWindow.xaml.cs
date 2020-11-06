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
    /// Логика взаимодействия для AddWorkerWindow.xaml
    /// </summary>
    public partial class AddWorkerWindow : Window
    {
        Repository temp_rep = new Repository();
        string Name_dep = null;
        public AddWorkerWindow(Repository repository)
        {
            temp_rep = repository;
            InitializeComponent();
        }

        public AddWorkerWindow(Repository repository, string department_name)
        {
            Name_dep = department_name;
            temp_rep = repository;
            InitializeComponent();
        }

        /// <summary>
        /// Обработка нажатия на кнопку "Добавить сотрудника"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButAddWorker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int IdWorker = (temp_rep.workers.Count > 0) ? temp_rep.workers[temp_rep.workers.Count - 1].ID + 1 : 0;
                if (int.Parse(BoxAge.Text) > 255)
                {
                    MessageBox.Show("СЛИШКОМ БОЛЬШОЙ ВОЗРАСТ", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (ComboPosition.Text == "Интерн")
                {
                    temp_rep.workers.Add(new Intern(BoxFirstName.Text, BoxLastName.Text, byte.Parse(BoxAge.Text),
                                                    ComboDepartment.Text, ComboPosition.Text, double.Parse(BoxSalary.Text)));
                    Close();
                }

                else if (ComboPosition.Text == "Рабочий")
                    if (BoxSalary.Text == string.Empty)
                        MessageBox.Show("Введите количество часов", "WARNING", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        temp_rep.workers.Add(new MainWorker(BoxFirstName.Text, BoxLastName.Text, byte.Parse(BoxAge.Text),
                                                                  ComboDepartment.Text, ComboPosition.Text, int.Parse(BoxHours.Text),
                                                                   double.Parse(BoxSalary.Text)));

                        Close();
                    }
                else if (ComboPosition.Text == "Директор")
                {
                    Director director = new Director(BoxFirstName.Text,
                                                      BoxLastName.Text, byte.Parse(BoxAge.Text),
                                                      "\0", ComboPosition.Text, 0);
                    director.Salary = director.RealSalary(temp_rep,director.ID);
                    temp_rep.workers.Add(director);
                    Close();
                }
                else if (ComboPosition.Text == "Менеджер")
                {
                    MainManager mainManager = new MainManager(BoxFirstName.Text,
                                                      BoxLastName.Text, byte.Parse(BoxAge.Text),
                                                      ComboDepartment.Text, ComboPosition.Text, 0);
                    mainManager.Salary = mainManager.RealSalary(temp_rep);
                    temp_rep.workers.Add(mainManager);
                    Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Выбор сотрудника в ComboBox

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
                if (!true_false)
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
        /// Получение списка департаментов во время инициализации окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDepartment_Initialized(object sender, EventArgs e)
        {
            for (int i = 0; i < temp_rep.departments.Count; i++)
                ComboDepartment.Items.Add(newItem: temp_rep.departments[i].Name);
            if ((Name_dep != null) && (Name_dep != "Организация"))
            {
                ComboDepartment.Text = Name_dep;
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Сохранить".
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
