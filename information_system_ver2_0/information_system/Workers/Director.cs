using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace information_system
{
    /// <summary>
    /// Класс - Директор
    /// </summary>
    class Director : Worker
    {
        /// <summary>
        /// Зарплата
        /// </summary>
        public override double Salary { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public override string Position { get { return "Директор"; } }
        public Director(string FirstName, string LastName, byte Age, string Departament, string Position, double Salary)
         : base(FirstName, LastName, Age, Departament)
        {
            this.Salary = Salary;
        }

        /// <summary>
        /// Пересчет зп У директора
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="ID_worker"></param>
        /// <returns></returns>
        public double RealSalary(Repository repository, int ID_worker)
        {
            double zp = 0;
            for (int i = 0; i < repository.workers.Count; i++)
            {
                if (repository.workers[i].ID != ID_worker)
                {
                    zp += repository.workers[i].Salary;
                }
            }
            if (zp * 0.15 > 1300)
            {
                zp = zp * 0.15;
            }
            else zp = 1300;

            return zp;
        }
    }
}
