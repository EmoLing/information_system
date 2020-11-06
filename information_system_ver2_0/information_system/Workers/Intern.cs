using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace information_system
{
    class Intern : Worker
    {
        /// <summary>
        /// Переопределение Salary и Position
        /// </summary>
        public override double Salary { get { return 500; } }

        /// <summary>
        /// Должность
        /// </summary>
        public override string Position { get { return "Интерн"; } }

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

        public Intern(string FirstName, string LastName, byte Age, string Departament, string Position, double Salary)
            : base(FirstName, LastName, Age, Departament)
        {
        }
    }
}
