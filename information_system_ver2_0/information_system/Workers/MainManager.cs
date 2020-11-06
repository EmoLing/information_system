using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace information_system
{
    /// <summary>
    /// Менеджер
    /// </summary>
    class MainManager : Worker
    {
        /// <summary>
        /// Зарплата
        /// </summary>
        public override double Salary { get; set; }
        /// <summary>
        /// Должность
        /// </summary>
        public override string Position { get { return "Менеджер"; } }
        public MainManager(string FirstName, string LastName, byte Age, string Departament, string Position, double Salary)
         : base(FirstName, LastName, Age, Departament)
        {
            this.Salary = Salary;
        }

        /// <summary>
        /// Подсчет зарплаты менеджера
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public double RealSalary(Repository repository)
        {
           double zp = 0;

            //Подсчет зп у всех интернов/рабочих
            for (int i = 0; i < repository.workers.Count; i++)
            {
                if ((this.Department == repository.workers[i].Department) &&
                    ((repository.workers[i].Position == "Интерн") || (repository.workers[i].Position == "Рабочий")))
                {
                    zp += repository.workers[i].Salary;
                }
            }
            //int worker_dep_id = 0;
            //int worker_dep_childId = 0;
            //for (int i = 0; i < repository.departments.Count; i++)
            //{
            //    if(this.Department == repository.departments[i].Name)
            //    {
            //        worker_dep_id = repository.departments[i].ID;
            //        worker_dep_childId = repository.departments[i].childID;
            //    }
            //}

            //for (int i = 0; i < repository.departments.Count; i++)
            //{
            //    if ((worker_dep_id == repository.departments[i].ID) && 
            //        (worker_dep_childId < repository.departments[i].childID))
            //    {
            //        for (int j = 0; j < repository.workers.Count; j++)
            //        {
            //            if (repository.workers[j].Department == repository.departments[i].Name)
            //                zp += repository.workers[j].Salary;
            //        }
            //    }
            //}
            for (int i = 0; i < repository.departments.Count; i++)
            {
                if (repository.departments[i].Name == this.Department)
                {
                    zp += Check_Child_Dep(i,repository,this.Department);
                    break;
                }
            }

            zp *= 0.15;

            if (zp <= 1300)
                zp = 1300;

            return zp;
        }

        private double Check_Child_Dep(int ID_parent, Repository repository,string NameMainDep)
        {
            double temp_zp = 0;
            for (int i = 0; i < repository.departments.Count; i++)
            {
                //if ((repository.departments[i].Id_Node_Dep == 0) && (repository.departments[repository.departments[i].Id_Node_Dep].Name != NameMainDep))
                //{
                //    continue;
                //}
                if ((repository.departments[i].Id_Node_Dep == ID_parent) && (i != ID_parent))
                {
                    for (int j = 0; j < repository.workers.Count; j++)
                    {
                        if (repository.departments[i].Name == repository.workers[j].Department)
                        {
                            temp_zp += repository.workers[j].Salary;
                        }
                    }
                    temp_zp += Check_Child_Dep(i, repository, NameMainDep);
                }
            }
            return temp_zp;
        }
    }
}
