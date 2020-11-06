using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace information_system
{
    public class Repository
    {
        /// <summary>
        /// Колеция сотрудников
        /// </summary>
        public ObservableCollection<Worker> workers { get; set; }
        /// <summary>
        /// Коллекция отделов
        /// </summary>
        public ObservableCollection<Department> departments { get; set; }

        public Repository ()
        {
            workers = new ObservableCollection<Worker>();
            departments = new ObservableCollection<Department>();
        }
        public Worker this[string Position]
        {
            get
            {
                Worker t = null;
                foreach (var e in this.workers)
                {
                    if (e.Position == Position) { t = e; break; }
                }
                return t;
            }
        }

    }
}
