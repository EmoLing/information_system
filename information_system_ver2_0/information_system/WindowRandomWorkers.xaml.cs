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
    /// Логика взаимодействия для WindowRandomWorkers.xaml
    /// </summary>
    public partial class WindowRandomWorkers : Window
    {
        Repository repository;
        public WindowRandomWorkers(Repository main_repository)
        {
            repository = main_repository;
            InitializeComponent();
        }

        /// <summary>
        /// Запуск процесса генерации сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButGoToMass_Click(object sender, RoutedEventArgs e)
        {
            List<string> Workers_FirstNames = new List<string>();
            Workers_FirstNames = LoadNames("Names.txt");
            List<string> Workers_LastNames = new List<string>();
            Workers_LastNames = LoadNames("LastNames.txt");
            int count_workers = 0;
            try
            {
                count_workers = int.Parse(RandomBox.Text); //количество рабочих
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            Random r = new Random();
            Random random_spec = new Random(); //специальность
            Random random_name = new Random();
            Random random_dep = new Random();

            while (count_workers > 0)
            {
                int count = repository.workers.Count;
                switch (random_spec.Next(1, 3))
                {
                    case 1:
                        {
                            Worker worker = new Intern($"{Workers_FirstNames[random_name.Next(Workers_FirstNames.Count)]}",
                                                        $"{Workers_LastNames[random_name.Next(Workers_LastNames.Count)]}",
                                                        byte.Parse(r.Next(18, 66).ToString()),
                                                        repository.departments[random_dep.Next(repository.departments.Count)].Name,
                                                        "Интерн", 500);
                            repository.workers.Add(worker);
                            break;
                        }
                    case 2:
                        {
                            int hours = r.Next(99, 1001);
                            Worker worker = new MainWorker(
                                $"{Workers_FirstNames[random_name.Next(Workers_FirstNames.Count)]}",
                                $"{Workers_LastNames[random_name.Next(Workers_LastNames.Count)]}",
                                byte.Parse(r.Next(1, 65).ToString()),
                                repository.departments[random_dep.Next(repository.departments.Count)].Name,
                                "Рабочий", hours, hours * 12);
                            repository.workers.Add(worker);
                            break;
                        }
                    default:
                        break;
                }
                count_workers--;
            }
            Close();
        }

        /// <summary>
        /// Дессериализация дефолтных имен/фамилий
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<string> LoadNames(string path)
        {
            List<string> Workers_Names = new List<string>();
            string name = string.Empty;
            try
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    name = streamReader.ReadToEnd();
                }
                var names = name.Split(',', ' ');
                for (int i = 0; i < names.Length; i++)
                {
                    Workers_Names.Add(names[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return Workers_Names;

        }

    }
}
