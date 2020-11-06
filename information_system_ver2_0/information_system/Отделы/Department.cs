using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace information_system
{
    public class Department
    {
        #region Свойства
        /// <summary>
        /// Статическое поле staticId
        /// </summary>
        private static int staticUniqId;

        /// <summary>
        /// Статический конструктор
        /// </summary>
        static Department()
        {
            staticUniqId = -1;
        }

        /// <summary>
        /// Статический метод возвращающий следующие Id
        /// </summary>
        /// <returns></returns>
        private static int NextUniqId()
        {
            staticUniqId++;
            return staticUniqId;
        }

        /// <summary>
        /// ID основного отдела
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime DateOfCreate { get; set; }

        /// <summary>
        /// Родительский ID (возможно лишний параметр)
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// Отдел узла к которому привязывается
        /// </summary>
        public int Id_Node_Dep { get; set; }

        /// <summary>
        /// ID ребенка
        /// По старшенству 
        /// 0-главный родитель 
        /// 1 - родитель для 2, 3 и тд. и ребенок для 0
        /// 2 - родитель для 3, 4 и тд. и ребенок для 1 и 2
        /// и т.п.
        /// </summary>
        public int childID { get; set; }
        public int UniqID { get; set; }
        #endregion

        #region Конструкторы

        public Department(int ID,string Name, DateTime DateOfCreate, int ParentID, int childID)
        {
            this.ID = ID;
            this.Name = Name;
            this.DateOfCreate = DateOfCreate;
            this.ParentID = ParentID;
            this.childID = childID;
            this.UniqID = NextUniqId();
        }

        public Department(int ID, string Name, DateTime DateOfCreate, int ParentID, int childID, int Id_Node_Dep)
        {
            this.ID = ID;
            this.Name = Name;
            this.DateOfCreate = DateOfCreate;
            this.ParentID = ParentID;
            this.childID = childID;
            this.Id_Node_Dep = Id_Node_Dep;
            this.UniqID = NextUniqId();
        }
        #endregion
    }
}
