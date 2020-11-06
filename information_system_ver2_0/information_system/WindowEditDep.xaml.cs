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
    /// Логика взаимодействия для WindowEditDep.xaml
    /// </summary>
    public partial class WindowEditDep : Window
    {
        private Repository temp_rep;
        private TreeViewItem viewItem = new TreeViewItem();

        public WindowEditDep(Repository repository, TreeViewItem viewItem)
        {
            temp_rep = repository;
            this.viewItem = viewItem;
            InitializeComponent();
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

                if (viewItem == null)
                {
                    ComboDepartment.Text = temp_rep.departments[temp_rep.departments.Count - 1].Name;
                }
                else
                {
                    ComboDepartment.Text = viewItem.Header.ToString();
                }
            }
            else

            {
                MessageBox.Show("Departments not found", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                CheckMainDep.IsChecked = false;
            }

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
        private void ButEditDep_Click(object sender, RoutedEventArgs e)
        {
            Department current_dep_temp = null;
            int index_dep = GetDep(out current_dep_temp);
            Department current_dep = current_dep_temp;
            if (index_dep == -1)
            {
                MessageBox.Show("Отдел не найден", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string old_name = current_dep.Name;
            int old_id = current_dep.ID;
            try
            {

                int n = temp_rep.departments.Count; //количество отделов
                temp_rep.departments[index_dep].Name = null;
                if ((BoxName.Text != String.Empty) &
                    (BoxDate.Text != String.Empty) & CheckName(n, temp_rep, BoxName.Text)) //проверка на пустоту строк 
                                                                                           //и уникальное название
                {
                    if (TextIsDate(BoxDate.Text))
                    {
                        if (CheckMainDep.IsChecked == false)
                        {
                            current_dep.Name = BoxName.Text;
                            current_dep.DateOfCreate = Convert.ToDateTime(BoxDate.Text);
                        }
                        else
                        {
                            if (ComboDepartment.SelectedItem == null)

                            {
                                MessageBox.Show("Отдел не выбран!");
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
                                        break;
                                    }
                                }

                                current_dep.ID = ID_dep;
                                current_dep.Name = BoxName.Text;
                                current_dep.DateOfCreate = Convert.ToDateTime(BoxDate.Text);
                                current_dep.ParentID = ID_Parent;
                                current_dep.childID = ID_child;
                                current_dep.Id_Node_Dep = ID_in_Mass;
                            }

                        }
                        temp_rep.departments[index_dep] = current_dep;

                        for (int i = 0; i < temp_rep.workers.Count; i++)
                        {
                            if (temp_rep.workers[i].Department == old_name)
                            {
                                temp_rep.workers[i].Department = current_dep.Name;
                            }
                        }

                        UpdateNodes(current_dep.UniqID, temp_rep.departments.Count, current_dep);
                        this.Close();
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
            }
        }


        /// <summary>
        /// Обновление связей
        /// </summary>
        /// <param name="index_dep">Текущий индекс</param>
        /// <param name="count_deps">Количество отделов</param>
        /// <param name="current_dep">Текущий отдел</param>
        private void UpdateNodes(int index_dep, int count_deps, Department current_dep)
        {
            for (int k = 0; k < temp_rep.departments.Count; k++)
            {
                if (current_dep.Name == temp_rep.departments[k].Name)
                {
                    for (int i = 0; i < temp_rep.departments.Count; i++)
                    {
                        if ((temp_rep.departments[i].Id_Node_Dep == index_dep))
                        {
                            for (int j = 0; j < temp_rep.departments.Count; j++)
                            {
                                if ((temp_rep.departments[i].Id_Node_Dep == temp_rep.departments[j].Id_Node_Dep)
                                    && (temp_rep.departments[i].childID == temp_rep.departments[j].childID)
                                    && (temp_rep.departments[i].Name != temp_rep.departments[j].Name))
                                {
                                    temp_rep.departments[i].ParentID++;
                                }
                            }
                            count_deps--;
                            //if (count_deps < 0)
                            //    break;

                            temp_rep.departments[i].ID = current_dep.ID;
                            temp_rep.departments[i].childID = current_dep.childID + 1;
                            UpdateNodes(temp_rep.departments[i].UniqID, count_deps, temp_rep.departments[i]);
                        }
                    }
                    break;
                }
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
        /// Открытие окна календаря
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

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {


            if ((viewItem != null) && (viewItem.Header.ToString() != "Организация"))
            {
                Department department_current = null;
                GetDep(out department_current);

                List<int> ListUniqId = new List<int>();
                ListUniqId.Add(department_current.UniqID);
                GetOtherDeps(temp_rep, department_current, ref ListUniqId);


                for (int i = 0; i < temp_rep.departments.Count; i++)
                {
                    if (!ListUniqId.Contains(temp_rep.departments[i].UniqID))
                    {
                        ComboDepartment.Items.Add(newItem: temp_rep.departments[i].Name);
                    }
                }
                

                BoxName.Text = department_current.Name;
                BoxDate.Text = department_current.DateOfCreate.ToShortDateString();
                if (department_current.Id_Node_Dep > -1)
                {
                    ComboDepartment.SelectedItem = temp_rep.departments[department_current.Id_Node_Dep].Name;
                }
                else
                    CheckMainDep.IsChecked = false;
            }
        }

        /// <summary>
        /// Получение Отделов, к которым нельзя будет привязать отдел
        /// </summary>
        /// <param name="temp_rep">Репозиторий</param>
        /// <param name="UniqId">Уникальный Айди</param>
        /// <param name="ListUniqId">Все айдишники запрещенных отделов</param>
        private void GetOtherDeps(Repository temp_rep, Department department_current, ref List<int> ListUniqId)
        {
            for (int i = 0; i < temp_rep.departments.Count; i++)
            {
                if (temp_rep.departments[i].Id_Node_Dep == department_current.UniqID && temp_rep.departments[i].ID == department_current.ID)
                {
                    ListUniqId.Add(temp_rep.departments[i].UniqID);
                    GetOtherDeps(temp_rep, temp_rep.departments[i], ref ListUniqId);
                }
            }
        }

        /// <summary>
        /// Получение отдела
        /// </summary>
        /// <param name="department_current">Запись полученного отдела</param>
        /// <returns>айдишник отдела</returns>
        private int GetDep(out Department department_current)
        {
            department_current = null;
            for (int i = 0; i < temp_rep.departments.Count; i++)
            {
                if (viewItem.Header.ToString() == temp_rep.departments[i].Name)
                {
                    department_current = temp_rep.departments[i];
                    return i;
                }
            }
            return -1;
        }
    }
}
