using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using System.ComponentModel;

namespace information_system
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Repository repository = new Repository();

        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Добавление отдела в TreeViewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeAdd(object sender, RoutedEventArgs e)
        {
            WindowDepartment windowDepartment = null;

            if (TreeDepartments.SelectedItem == null)
                windowDepartment = new WindowDepartment(repository);
            else
                windowDepartment = new WindowDepartment(repository, TreeDepartments.SelectedItem as TreeViewItem);

            int temp_count = repository.departments.Count;

            windowDepartment.Owner = this;
            this.IsEnabled = false;
            if (windowDepartment.ShowDialog() == true)
            {
                windowDepartment.Show();
                windowDepartment.Activate();
            }
            this.IsEnabled = true;
            if (repository.departments.Count > temp_count)
                TreeDepartments.Items.Add(TreeDepartments_PreInitialized());

        }


        /// <summary>
        /// Удаление отдела в TreeViewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeDelete(object sender, RoutedEventArgs e)
        {
            if (TreeDepartments.SelectedItem != null)
            {
                string name_delete_dep = (TreeDepartments.SelectedItem as TreeViewItem).Header.ToString(); //Название удаляемого отдела
                Department dep_delete = null; //Отдел, который будет удален
                Dictionary<int,string> MassUniqIdDelete = new Dictionary<int, string>(); //Словарь уникальных значений и названий основного и потомственных отделов

                //Получение удаляемого отдела
                foreach (var item in repository.departments)
                {
                    if (item.Name == name_delete_dep)
                    {
                        dep_delete = item;
                        break;
                    }
                }

                MassUniqIdDelete.Add(dep_delete.UniqID, dep_delete.Name);

                //Поиск потомственных отделов
                MassId(MassUniqIdDelete, dep_delete);

                //Поиск сотрудников, у которых удаляются отделы
                //И изменение поля Отделы у сотрудников
                for (int i = 0; i < repository.workers.Count; i++)
                {
                    if (MassUniqIdDelete.ContainsValue(repository.workers[i].Department))
                    {
                        repository.workers[i].Department = "ОТДЕЛ УДАЛЕН";
                    }
                }

                //Удаление отделов 
                int count = 0;
                while (true)
                {
                    if (MassUniqIdDelete.ContainsKey(repository.departments[count].UniqID))
                    {
                        repository.departments.Remove(repository.departments[count]);
                        count = 0;
                    }
                    count++;
                    if (count >= repository.departments.Count) break;
                 }

                ListView_Update(repository);
                TreeDepartments_Update(sender, e);
            }

        }

        /// <summary>
        /// Поиск потомственных отделов, которые будут удалены
        /// </summary>
        /// <param name="MassUniqIdDelete">Сюда записывается ID + название отдела</param>
        /// <param name="temp_dep_delete">Родительский отдел</param>
        private void MassId (Dictionary<int, string> MassUniqIdDelete, Department temp_dep_delete)
        {
            foreach (var item in repository.departments)
            {
                if (item.Id_Node_Dep == temp_dep_delete.UniqID)
                {
                    MassUniqIdDelete.Add(item.UniqID,item.Name);
                    MassId(MassUniqIdDelete, item);
                }
            }
        }

        /// <summary>
        /// Обработка нажатия на кнопку "Удалить сотрудника"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteWorker_Click(object sender, RoutedEventArgs e)
        {
            if (listview_workers.SelectedValue == null)
                MessageBox.Show("Не выбран сотрудник!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                MessageBoxResult result = MessageBox.Show($"Сотрудник {repository.workers[listview_workers.SelectedIndex].FirstName}" +
                                                          $" {repository.workers[listview_workers.SelectedIndex].LastName} " +
                                                          $"будет удален", "WARMIMG", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    repository.workers.Remove(repository.workers[listview_workers.SelectedIndex]);
                    listview_workers.Items.Remove(listview_workers.SelectedItem);
                    for (int i = 0; i < repository.workers.Count; i++)
                    {
                        if (repository.workers[i] is MainManager)
                            repository.workers[i].Salary = (repository.workers[i] as MainManager).RealSalary(repository);
                        else if (repository.workers[i] is Director)
                            repository.workers[i].Salary = (repository.workers[i] as Director).RealSalary(repository, repository.workers[i].ID);
                    }
                    listview_workers.Items.Clear();
                    for (int i = 0; i < repository.workers.Count; i++)
                    {
                        listview_workers.Items.Add(repository.workers[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Обработка нажатия на кнопку "Добавить нового сотрудинка"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewWorker_Click(object sender, RoutedEventArgs e)
        {
            if (repository.departments.Count == 0)
                MessageBox.Show("СОЗДАЙТЕ ОТДЕЛ", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                AddWorkerWindow addWorkerWindow = new AddWorkerWindow(repository);

                int temp_count = repository.workers.Count;

                addWorkerWindow.Owner = this;
                this.IsEnabled = false;
                if (addWorkerWindow.ShowDialog() == true)
                {
                    addWorkerWindow.Show();
                    addWorkerWindow.Activate();
                }
                this.IsEnabled = true;
                if (repository.workers.Count > temp_count)
                {
                    listview_workers.Items.Add(newItem: repository.workers[repository.workers.Count - 1]);
                    listview_workers.Items.Clear();
                    for (int i = 0; i < repository.workers.Count; i++)
                    {
                        listview_workers.Items.Add(repository.workers[i]);
                    }

                    for (int i = 0; i < repository.workers.Count; i++)
                    {
                        if (repository.workers[i] is MainManager)
                            repository.workers[i].Salary = (repository.workers[i] as MainManager).RealSalary(repository);
                        else if (repository.workers[i] is Director)
                            repository.workers[i].Salary = (repository.workers[i] as Director).RealSalary(repository, repository.workers[i].ID);
                    }
                }
            }
        }

        /// <summary>
        /// Обработка нажатия на кнопку "Редактировать сотрудника"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditWorker_Click(object sender, RoutedEventArgs e)
        {
            if (listview_workers.SelectedValue == null)
                MessageBox.Show("Не выбран сотрудник!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                EditWorkerWindow editWorkerWindow = new EditWorkerWindow(repository, listview_workers.Items.IndexOf(listview_workers.SelectedItem));

                editWorkerWindow.Owner = this;
                this.IsEnabled = false;
                if (editWorkerWindow.ShowDialog() == true)
                {
                    editWorkerWindow.Show();
                    editWorkerWindow.Activate();
                }
                this.IsEnabled = true;
                listview_workers.Items.Clear();
                for (int i = 0; i < repository.workers.Count; i++)
                {
                    listview_workers.Items.Add(repository.workers[i]);
                }
                for (int i = 0; i < repository.workers.Count; i++)
                {
                    if (repository.workers[i] is MainManager)
                        repository.workers[i].Salary = (repository.workers[i] as MainManager).RealSalary(repository);
                    else if (repository.workers[i] is Director)
                        repository.workers[i].Salary = (repository.workers[i] as Director).RealSalary(repository, repository.workers[i].ID);
                }
            }
        }

        /// <summary>
        /// Показ департаментов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButShowDep_Click(object sender, RoutedEventArgs e)
        {
            WinShowDep winShowDep = new WinShowDep(repository);
            winShowDep.Show();
        }

        /// <summary>
        /// Нажатие на кнопку Сериализация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSerializ_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(repository);
            File.WriteAllText("_repositury.json", json);
            MessageBox.Show("Готово", "Complite", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        /// <summary>
        /// Загрузка приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            MenuItemAutoLoad.IsChecked = AutoLoad_settings();
            if (AutoLoad_settings())
            {
                DeserializeRepository(repository, "_repositury.json");
            }
            MessageBox.Show("Прочтите справку:\nМеню->Справка", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Настройки автозагрузки
        /// </summary>
        /// <returns></returns>
        private bool AutoLoad_settings()
        {
            string json = File.ReadAllText("settings.json");
            var settings = JObject.Parse(json);
            bool set = bool.Parse(settings["AutoLoad"].ToString());
            return set;
        }

        /// <summary>
        /// Меню -> Выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButExit(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        /// <summary>
        /// Открытие файла с выгруженной информационной системой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "json files (*.json)|*.json";
            if (openFile.ShowDialog() == true)
            {
                var my_file = openFile.FileName;
                repository.departments.Clear();
                repository.workers.Clear();
                DeserializeRepository(repository, my_file);
            }


        }

        /// <summary>
        /// Десериализация репозиториев
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="file"></param>
        private void DeserializeRepository(Repository repository, string file)
        {
            try
            {

                #region Десериализация_Отделов

                string json = File.ReadAllText(file);
                var temp_dep_json = JObject.Parse(json)["departments"].ToArray();
                foreach (var item in temp_dep_json)
                {
                    int itemID = int.Parse(item["ID"].ToString());
                    string itemName = item["Name"].ToString();
                    DateTime itemDateOfCreate = DateTime.Parse(item["DateOfCreate"].ToString());
                    int itemCOW = int.Parse(item["ParentID"].ToString());
                    int itemChilId = int.Parse(item["childID"].ToString());
                    int itemId_Node_Dep = int.Parse(item["Id_Node_Dep"].ToString());
                    repository.departments.Add(new Department(itemID, itemName, itemDateOfCreate, itemCOW, itemChilId, itemId_Node_Dep));
                }


                //for (int i = 0; i < repository.departments.Count; i++)
                //{
                //    TreeDepartments.Items.Add(repository.departments[i].Name);
                //}
                TreeDepartments.Items.Add(TreeDepartments_PreInitialized());
                #endregion

                #region Десериализация_Сотрудников

                var temp_workers_json = JObject.Parse(json)["workers"].ToArray();
                foreach (var item in temp_workers_json)
                {
                    int itemID = int.Parse(item["ID"].ToString());
                    string itemFirstName = item["FirstName"].ToString();
                    string itemLastName = item["LastName"].ToString();
                    byte itemAge = byte.Parse(item["Age"].ToString());
                    string itemDepartment = item["Department"].ToString();
                    string itemPosition = item["Position"].ToString();
                    double itemSalary = double.Parse(item["Salary"].ToString());
                    if (itemPosition == "Рабочий")
                    {
                        int itemCountHours = int.Parse(item["CountHours"].ToString());
                        repository.workers.Add(new MainWorker(itemFirstName, itemLastName,
                                                              itemAge, itemDepartment, itemPosition,
                                                              itemCountHours, itemSalary));
                    }

                    else if (itemPosition == "Интерн")
                        repository.workers.Add(new Intern(itemFirstName, itemLastName,
                                                          itemAge, itemDepartment, itemPosition,
                                                          itemSalary));
                    else if (itemPosition == "Директор")
                        repository.workers.Add(new Director(itemFirstName, itemLastName,
                                                            itemAge, itemDepartment, itemPosition,
                                                            itemSalary));
                    else if (itemPosition == "Менеджер")
                        repository.workers.Add(new MainManager(itemFirstName, itemLastName,
                                                               itemAge, itemDepartment, itemPosition,
                                                               itemSalary));
                }

                for (int i = 0; i < repository.workers.Count; i++)
                {
                    listview_workers.Items.Add(repository.workers[i]);
                }
                MessageBox.Show("Загрузка прошла успешно!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Включить автозагрузку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAutoLoad_Checked(object sender, RoutedEventArgs e)
        {
            Dictionary<string, bool> AutoLoad = new Dictionary<string, bool>();
            AutoLoad.Add("AutoLoad", true);
            string json = JsonConvert.SerializeObject(AutoLoad);
            File.WriteAllText("settings.json", json);
            MessageBox.Show("Автозагрузка включена", "Complite", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        /// <summary>
        /// Выключить автозагрузку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAutoLoad_Unchecked(object sender, RoutedEventArgs e)
        {
            Dictionary<string, bool> AutoLoad = new Dictionary<string, bool>();
            AutoLoad.Add("AutoLoad", false);
            string json = JsonConvert.SerializeObject(AutoLoad);
            File.WriteAllText("settings.json", json);
            MessageBox.Show("Автозагрузка выключена", "Complite", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        /// <summary>
        /// Подробная информация о работнике
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewWorker_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listview_workers.SelectedValue != null)
            {
                WIndowShowWorker showWorker = new WIndowShowWorker(repository, listview_workers.Items.IndexOf(listview_workers.SelectedItem));
                showWorker.Show();
            }
        }

        /// <summary>
        /// Информация о системе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButFAQ_Click(object sender, RoutedEventArgs e)
        {
            WinFAQ winFAQ = new WinFAQ();
            winFAQ.Show();
        }

        /// <summary>
        /// TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 


        //Апдейт через правую кнопку мыши в TreeView (было сделано для тестирования)
        private void TreeDepartments_Update(object sender, EventArgs e)
        {
            TreeDepartments.Items.Add(TreeDepartments_PreInitialized());
        }

        /// <summary>
        /// Инициализация TreeView
        /// </summary>
        /// <returns></returns>
        private TreeViewItem TreeDepartments_PreInitialized()
        {
            TreeDepartments.Items.Clear();

            TreeViewItem item = new TreeViewItem();
            TreeViewItem test_item = new TreeViewItem();
            TreeViewItem test_item_dva = new TreeViewItem();

            item.Header = "Организация";
            item.IsExpanded = true;

            for (int i = 0; i < repository.departments.Count; i++)
            {
                if (repository.departments[i].childID == 0)
                {

                    test_item_dva.Header = repository.departments[i].Name;
                    test_item.IsExpanded = true;
                    test_item_dva.IsExpanded = true;

                    var temp = GetTreeNodes(test_item, 0, test_item_dva.Header.ToString());
                    for (int j = 0; j < temp.Count; j++)
                    {
                        test_item_dva.Items.Add(temp[j]);
                    }
                    item.Items.Add(test_item_dva);

                    test_item = new TreeViewItem();
                    test_item_dva = new TreeViewItem();
                }
            }
            return item;
        }


        /// <summary>
        /// Функция добавление подъотделов к главным отделам с childID = 0
        /// </summary>
        private ObservableCollection<TreeViewItem> GetTreeNodes(TreeViewItem item, int n, string mainDep)
        {
            ObservableCollection<TreeViewItem> treeViewItems = new ObservableCollection<TreeViewItem>();
            int id_dep = 0;
            int child_id_dep = 0;
            //Определение ID отдела
            for (int i = 0; i < repository.departments.Count; i++)
            {
                if (mainDep == repository.departments[i].Name)
                {
                    id_dep = repository.departments[i].ID;
                    child_id_dep = repository.departments[i].childID;
                    break;
                }
            }

            //вычисление максимального ID ребенка
            int[] mass_max = new int[repository.departments.Count];
            int kol_childid = 0;
            for (int i = 0; i < repository.departments.Count; i++)
            {
                if (id_dep == repository.departments[i].ID)
                {
                    mass_max[kol_childid] = repository.departments[i].childID;
                    kol_childid++;
                }
            }
            int max_child_id = mass_max.Max();

            TreeViewItem item_test_node = new TreeViewItem();
            int count = 0;
            for (int i = n; i < repository.departments.Count; i++)
            {
                bool check = false;
                var Current_Dep = repository.departments[i];

                if ((id_dep == Current_Dep.ID) && (mainDep != Current_Dep.Name) && (Current_Dep.childID - 1 == child_id_dep))
                {
                    item_test_node.Header = Current_Dep.Name;

                    check = true;

                }
                if (check)
                {
                    var reo_test = repository.departments;
                    AddNodesForTree(max_child_id, repository.departments[i].childID, item, reo_test, 0, Current_Dep);

                    item_test_node = new TreeViewItem();
                    treeViewItems.Add(item);
                    treeViewItems[count].Header = Current_Dep.Name;
                    item = new TreeViewItem();
                    count++;
                }
            }
            for (int i = 0; i < treeViewItems.Count; i++)
            {
                treeViewItems[i].IsExpanded = true;
            }
            return treeViewItems;
        }


        /// <summary>
        /// Рекурсия по добавлению узлов TreeView
        /// </summary>
        /// <param name="Maxchild_ID">Максимальный потомственный айдишник</param>
        /// <param name="current_child_ID">Текущий потомственный айдишник</param>
        /// <param name="Node_current">Родитеский узел</param>
        /// <param name="reo_test">Коллекция отделов (можно было и не переносить ее)</param>
        /// <param name="i">Индекс текущего отдела</param>
        /// <param name="Current_Dep">Текущий отдел</param>

        private void AddNodesForTree(int Maxchild_ID, int current_child_ID, TreeViewItem Node_current, 
                                    ObservableCollection<Department> reo_test, int i, Department Current_Dep)
        {
            //Получение индексов элементов ID которых совпадаю с ID узла 
            List<int> MassID = new List<int>();
            int id = 0;

            for (int j = 0; j < repository.departments.Count; j++)
            {
                if (repository.departments[j].Name == Current_Dep.Name)
                {
                    id = Current_Dep.UniqID;
                    break;
                }
            }

            for (int j = 0; j < repository.departments.Count; j++)
            {
                if ((repository.departments[j].ID == Current_Dep.ID))
                {
                    if (repository.departments[j].Id_Node_Dep == id)
                    {
                        MassID.Add(j);
                    }
                }
            }

            for (int j = 0; j < MassID.Count; j++)
            {
                TreeViewItem Post_Node = new TreeViewItem();
                Post_Node.IsExpanded = true;
                //if (current_child_ID <= Maxchild_ID+1)
                //{
                current_child_ID++;
                Current_Dep = reo_test[MassID[j]];
                Post_Node.Header = reo_test[MassID[j]].Name;
                Node_current.Items.Add(Post_Node);
                AddNodesForTree(Maxchild_ID, current_child_ID, Post_Node, reo_test, id, Current_Dep);
                //}
            }
        }



        /// <summary>
        /// ListView -> Правая кнопка мыши -> Обновить
        /// Происходит восстановление ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickUpdate(object sender, RoutedEventArgs e)
        {
            ListView_Update(repository);
        }

        /// <summary>
        /// Меню -> Сохранить как...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSerializHOW_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "json files (*.json)|*.json";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(repository);
                File.WriteAllText($"{saveFileDialog1.FileName}", json);
                MessageBox.Show("Готово", "Complite", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        /// <summary>
        /// Меню -> Рандомная генерация работников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButRandomWorkers(object sender, RoutedEventArgs e)
        {
            WindowRandomWorkers windowRandom = new WindowRandomWorkers(repository);

            int temp_count = repository.workers.Count;

            windowRandom.Owner = this;
            this.IsEnabled = false;
            if (windowRandom.ShowDialog() == true)
            {
                windowRandom.Show();
                windowRandom.Activate();
            }
            this.IsEnabled = true;
            listview_workers.Items.Clear();
            for (int i = 0; i < repository.workers.Count; i++)
            {
                listview_workers.Items.Add(repository.workers[i]);
            }
        }

        /// <summary>
        /// Выбор отдела и показ его сотрудников в listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeDepartments_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                listview_workers.Items.Clear();
                if (TreeDepartments.SelectedItem != null)
                {
                    if ((TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() != "Организация")
                    {
                        if (CheckSeeOtherWorkers.IsChecked == false) Workers_In_Current_Dep();

                        else Workers_In_Current_And_Child_Deps();
                    }
                    else if ((TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() == "Организация")
                    {
                        for (int i = 0; i < repository.workers.Count; i++)
                        {

                            listview_workers.Items.Add(newItem: repository.workers[i]);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Рабочие в текущем отделе
        /// </summary>
        private void Workers_In_Current_Dep()
        {
            for (int i = 0; i < repository.workers.Count; i++)
            {
                if ((TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() == repository.workers[i].Department)
                {
                    listview_workers.Items.Add(newItem: repository.workers[i]);
                }
            }
        }

        /// <summary>
        /// Получение сотрудников из всех дочерних отделов
        /// </summary>
        private void Workers_In_Current_And_Child_Deps()
        {
            Dictionary<int, string> UniqIdAndName_Deps = new Dictionary<int, string>();
            Department current_dep = null;
            GetDep(out current_dep);
            UniqIdAndName_Deps.Add(current_dep.UniqID, current_dep.Name);
            GetCurrents_all_childs(current_dep, ref UniqIdAndName_Deps);

            for (int i = 0; i < repository.workers.Count; i++)
            {
                if (UniqIdAndName_Deps.ContainsValue(repository.workers[i].Department))
                {
                    listview_workers.Items.Add(newItem: repository.workers[i]);
                }
            }
        }

        /// <summary>
        /// Получение отдела
        /// </summary>
        /// <param name="department_current">Получаемый отдел</param>
        private void GetDep(out Department department_current)
        {
            department_current = null;
            for (int i = 0; i < repository.departments.Count; i++)
            {
                if ((TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() == repository.departments[i].Name)
                {
                    department_current = repository.departments[i];
                    break;
                }
            }
        }

        /// <summary>
        /// Получение дочерних отделов
        /// </summary>
        /// <param name="department">Родительский отдел</param>
        /// <param name="UniqIdAndName_Deps">Коллекция уникальных индексов и названий дочерних отделов</param>
        private void GetCurrents_all_childs (Department department, ref Dictionary<int, string> UniqIdAndName_Deps)
        {
            for (int i = 0; i < repository.departments.Count; i++)
            {
                if (repository.departments[i].Id_Node_Dep == department.UniqID && repository.departments[i].ID == department.ID)
                {
                    UniqIdAndName_Deps.Add(repository.departments[i].UniqID, repository.departments[i].Name);
                    GetCurrents_all_childs(repository.departments[i], ref UniqIdAndName_Deps);
                }
            }
        }
        /// <summary>
        /// Обновление списка сотрудников
        /// </summary>
        /// <param name="repository"></param>
        private void ListView_Update(Repository repository)
        {
            listview_workers.Items.SortDescriptions.Clear();

            listview_workers.Items.Clear();
            for (int i = 0; i < repository.workers.Count; i++)
            {
                listview_workers.Items.Add(newItem: repository.workers[i]);
            }
        }

        #region Сортировка по столбцам

        private bool bool_Number = true; //флаг ID
        private bool bool_FirstName = true; //флаг Имени
        private bool bool_LastName = true; // флаг Фамилии
        private bool bool_Age = true; //флаг Возраста
        private bool bool_Department = true; //флаг Отдела
        private bool bool_Position = true; //флаг Должности
        private bool bool_Salary = true; //флаг зарплаты

        /// <summary>
        /// Сортировка по выбранному столбцу
        /// </summary>
        /// <param name="Field">Поле</param>
        /// <param name="check">значение флага</param>
        /// <returns></returns>
        private bool SortColum(string Field, bool check)
        {
            listview_workers.Items.SortDescriptions.Clear();
            if (check)
            {
                listview_workers.Items.SortDescriptions.Add(new SortDescription(Field, ListSortDirection.Descending));
                check = false;
            }
            else
            {
                listview_workers.Items.SortDescriptions.Add(new SortDescription(Field, ListSortDirection.Ascending));
                check = true;
            }
            return check;
        }

        /// <summary>
        /// Сортировка по ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberColum_Click(object sender, RoutedEventArgs e)
        {
            bool_Number = SortColum("ID", bool_Number);

        }

        /// <summary>
        /// Сортировка по Имени
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstNameColum_Click(object sender, RoutedEventArgs e)
        {
            // bool_FirstName = SortColum("FirstName", bool_FirstName);
            listview_workers.Items.Clear();
            listview_workers.Items.SortDescriptions.Clear();
            List<Worker> workers = new List<Worker>();
            foreach (var item in repository.workers)
            {
                workers.Add(item);
            }
            workers.Sort(Worker.SortedBy(SortedCriterion.FirstName));
            foreach (var item in workers)
            {
                listview_workers.Items.Add(item);
            }
        }

        /// <summary>
        /// Сортировка по Фамилии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LastNameColum_Click(object sender, RoutedEventArgs e)
        {
            bool_LastName = SortColum("LastName", bool_LastName);
        }

        /// <summary>
        /// Сортировка по Возрасту
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AgeColum_Click(object sender, RoutedEventArgs e)
        {
            bool_Age = SortColum("Age", bool_Age);
        }

        /// <summary>
        /// Сортировка по Отделу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DepartmentColum_Click(object sender, RoutedEventArgs e)
        {
            bool_Department = SortColum("Department", bool_Department);
        }

        /// <summary>
        /// Сортировка по Должности
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionColum_Click(object sender, RoutedEventArgs e)
        {
            bool_Position = SortColum("Position", bool_Position);
        }

        /// <summary>
        /// Сортировка по Зарплате
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SalaryColum_Click(object sender, RoutedEventArgs e)
        {
            bool_Salary = SortColum("Salary", bool_Salary);
        }

        #endregion

        /// <summary>
        /// Редактирование Отдела
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeEdit(object sender, RoutedEventArgs e)
        {
            if (TreeDepartments.SelectedItem != null)
            {

                WindowEditDep windowEdit = null;

                if ((TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() == "Организация")
                {
                    MessageBox.Show("Выберите отдел!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                    windowEdit = new WindowEditDep(repository, TreeDepartments.SelectedItem as TreeViewItem);

                int temp_count = repository.departments.Count;

                windowEdit.Owner = this;
                this.IsEnabled = false;
                if (windowEdit.ShowDialog() == true)
                {
                    windowEdit.Show();
                    windowEdit.Activate();
                }
                this.IsEnabled = true;
                TreeDepartments.Items.Clear();
                TreeDepartments.Items.Add(TreeDepartments_PreInitialized());

                for (int i = 0; i < repository.workers.Count; i++)
                {
                    if (repository.workers[i] is MainManager)
                        repository.workers[i].Salary = (repository.workers[i] as MainManager).RealSalary(repository);
                    else if (repository.workers[i] is Director)
                        repository.workers[i].Salary = (repository.workers[i] as Director).RealSalary(repository, repository.workers[i].ID);
                }

            }
            else
                MessageBox.Show("Выберите отдел!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }


        /// <summary>
        /// Добавление сотрудника в отдел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWorkerInDep(object sender, RoutedEventArgs e)
        {
            if (TreeDepartments.SelectedItem != null && (TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() != "Организация")
            {
                AddWorkerWindow addWorkerWindow = new AddWorkerWindow(repository, (TreeDepartments.SelectedItem as TreeViewItem).Header.ToString());

                int temp_count = repository.workers.Count;

                addWorkerWindow.Owner = this;
                this.IsEnabled = false;
                if (addWorkerWindow.ShowDialog() == true)
                {
                    addWorkerWindow.Show();
                    addWorkerWindow.Activate();
                }
                this.IsEnabled = true;
                if (repository.workers.Count > temp_count)
                {
                    listview_workers.Items.Add(newItem: repository.workers[repository.workers.Count - 1]);
                    listview_workers.Items.Clear();
                    for (int i = 0; i < repository.workers.Count; i++)
                    {
                        listview_workers.Items.Add(repository.workers[i]);
                    }

                    for (int i = 0; i < repository.workers.Count; i++)
                    {
                        if (repository.workers[i] is MainManager)
                            repository.workers[i].Salary = (repository.workers[i] as MainManager).RealSalary(repository);
                        else if (repository.workers[i] is Director)
                            repository.workers[i].Salary = (repository.workers[i] as Director).RealSalary(repository, repository.workers[i].ID);
                    }
                }
            }
        }

        /// <summary>
        /// Крестик стоит на показ всех сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckSeeOtherWorkers_Checked(object sender, RoutedEventArgs e)
        {
            listview_workers.Items.Clear();
            if (TreeDepartments.SelectedItem != null && (TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() != "Организация")
            {
                Workers_In_Current_And_Child_Deps();
            }
            else
            {
                for (int i = 0; i < repository.workers.Count; i++)
                {

                    listview_workers.Items.Add(newItem: repository.workers[i]);

                }
            }

        }

        /// <summary>
        /// Крестик не стоит на показ всех сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckSeeOtherWorkers_Unchecked(object sender, RoutedEventArgs e)
        {
            listview_workers.Items.Clear();
            if (TreeDepartments.SelectedItem != null && (TreeDepartments.SelectedItem as TreeViewItem).Header.ToString() != "Организация")
            {
                Workers_In_Current_Dep();
            }
            else
            {
                for (int i = 0; i < repository.workers.Count; i++)
                {

                    listview_workers.Items.Add(newItem: repository.workers[i]);

                }
            }
        }
    }
}
