using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace information_system
{
    public enum SortedCriterion
    {
        ID,
        FirstName,
        LastName,
        Age,
        Department,
        Position,
        Salary
    }
    public abstract class Worker 
    {

        /// <summary>
        /// Статическое поле staticId
        /// </summary>
        private static int staticId;

        /// <summary>
        /// Статический конструктор
        /// </summary>
        static Worker()
        {
            staticId = 0;
        }

        /// <summary>
        /// Статический метод возвращающий следующие Id
        /// </summary>
        /// <returns></returns>
        private static int NextId()
        {
            staticId++;
            return staticId;
        }
        public int LastId()
        {
            staticId--;
            return staticId;
        }
        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Возраст
        /// </summary>
        public byte Age { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public virtual string Position { get; set; }

        /// <summary>
        /// Зарплата
        /// </summary>
        public virtual double Salary { get; set; }

        /// <summary>
        /// Конструкторы
        /// </summary>
        /// <param name="ID">Порядковый номер</param>
        /// <param name="FirstName">Имя</param>
        /// <param name="LastName">Фамилия</param>
        /// <param name="Age">Возраст</param>
        /// <param name="Department">Отдел</param>
        /// <param name="Position">Должность</param>
        /// <param name="Salary">Зарплата</param>
        protected Worker (string FirstName, string LastName, byte Age, string Department, string Position, double Salary)
        {
            this.ID = Worker.NextId();
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Age = Age;
            this.Department = Department;
            this.Position = Position;
            this.Salary = Salary;
        }

        protected Worker(string FirstName, string LastName, byte Age, string Department)
        {
            this.ID = Worker.NextId();
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Age = Age;
            this.Department = Department;
        }



        /// <summary>
        /// Сортировка по ID
        /// </summary>
        private class SortByID : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;

                if (X.ID == Y.ID) return 0;
                else if (X.ID > Y.ID) return 0;
                else return -1;
            }
        }

        /// <summary>
        /// Сортировка по Имени
        /// </summary>
        private class SortByName : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;

                return String.Compare(X.FirstName, Y.FirstName);
            }
        }

        /// <summary>
        /// Сортировка по фамилии
        /// </summary>
        private class SortByLastName : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;

                return String.Compare(X.LastName, Y.LastName);
            }
        }
        private class SortByAge : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;

                if (X.Age == Y.Age) return 0;
                else if (X.Age > Y.Age) return 0;
                else return -1;
            }
        }

        /// <summary>
        /// Сортировка по отделам
        /// </summary>
        private class SortByDepartment : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;

                return String.Compare(X.Department, Y.Department);
            }
        }

        /// <summary>
        /// Сортировка по Должности
        /// </summary>
        private class SortByPosition : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;

                return String.Compare(X.Position, Y.Position);
            }
        }

        /// <summary>
        /// Сортировка по зарплате
        /// </summary>
        private class SortBySalary : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;

                if (X.Salary == Y.Salary) return 0;
                else if (X.Salary > Y.Salary) return 0;
                else return -1;
            }
        }
        public static IComparer<Worker> SortedBy(SortedCriterion Criterion)
        {
            switch (Criterion)
            {
                case SortedCriterion.ID:
                    return new SortByID();
                case SortedCriterion.FirstName:
                    return new SortByName();
                case SortedCriterion.LastName:
                    return new SortByLastName();
                case SortedCriterion.Age:
                    return new SortByAge();
                case SortedCriterion.Department:
                    return new SortByDepartment();
                case SortedCriterion.Position:
                    return new SortByPosition();
                case SortedCriterion.Salary:
                    return new SortBySalary();
                default:
                    return new SortByID();
            }

        }
    }
}
