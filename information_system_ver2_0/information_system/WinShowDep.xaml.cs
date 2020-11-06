using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для WinShowDep.xaml
    /// </summary>
    public partial class WinShowDep : Window
    {
        Repository temp_rep = new Repository();
        public WinShowDep(Repository repository)
        {
            temp_rep = repository;
            InitializeComponent();
        }

        /// <summary>
        /// Загрузка списка отделов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listDep_Initialized(object sender, EventArgs e)
        {
            for (int i = 0; i < temp_rep.departments.Count; i++)
                listDep.Items.Add(newItem: temp_rep.departments[i]);
            listDep.Items.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            listDep.Items.SortDescriptions.Add(new SortDescription("ParentID", ListSortDirection.Ascending));
            listDep.Items.SortDescriptions.Add(new SortDescription("childID", ListSortDirection.Ascending));
        }

        /// <summary>
        /// Показ работников отдела
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButShowWorkers_Click(object sender, RoutedEventArgs e)
        {
            listWorker.Items.Clear();

            var Selected_Dep_Index = listDep.SelectedItem;
            if (Selected_Dep_Index != null)
            {
                for (int i = 0; i < temp_rep.workers.Count; i++)
                {
                    if ((Selected_Dep_Index as Department).Name == temp_rep.workers[i].Department)
                    {
                        listWorker.Items.Add(newItem: temp_rep.workers[i]);
                    }
                }
            }
            else if (Selected_Dep_Index == null)
            {
                MessageBox.Show("ВЫ НЕ ВЫБРАЛИ ОТДЕЛ!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
