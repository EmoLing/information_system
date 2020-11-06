using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace information_system
{
    /// <summary>
    /// Рабочий
    /// </summary>
    class MainWorker : Worker
    {
        /// <summary>
        /// Количество часов
        /// </summary>
        public int CountHours { get; }

        /// <summary>
        /// Зарплата
        /// </summary>
        public override double Salary
        { get { return CountHours * 12; } }

        /// <summary>
        /// Должность
        /// </summary>
        public override string Position
        { get { return "Рабочий"; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Number">Номер</param>
        /// <param name="FirstName">Имя</param>
        /// <param name="LastName">Фамилия</param>
        /// <param name="Age">Возраст</param>
        /// <param name="Departament">Отдел</param>
        /// <param name="Position">Должность</param>
        /// <param name="Salary">Зарплата</param>
        public MainWorker(string FirstName, string LastName, byte Age, string Departament, string Position, int CountHours, double Salary)
            : base(FirstName, LastName, Age, Departament)
        {
            this.CountHours = CountHours;
        }
    }
}
