using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Homework_11
{
    public class Department
    {
        #region Поля и свойства класса
        /// <summary>
        /// Флаг для отметки корректности рассчета ЗП руководителя департамента и зарплатного фонда
        /// </summary>
        public bool SalaryCalculatedFlag { get; set; }
        
        /// <summary>
        /// Список всех Id существующих департаментов
        /// </summary>
        private static List<int> AllDepartmentsId { get; set; }

        /// <summary>
        /// Id департамента
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название департамента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id руководителя департамента
        /// </summary>
        public int ChiefId { get; set; }

        /// <summary>
        /// Список Id подчиненных департаментов
        /// </summary>
        public List<int> SlaveDepartmentId { get; set; }

        /// <summary>
        /// Список Id сотрудников департамента
        /// </summary>
        public List<int> DepartmentEmployeesId { get; set; }

        /// <summary>
        /// Id вышестоящего департамента
        /// </summary>
        public int HigherDepartmentId { get; set; }

        /// <summary>
        /// Зарплатный фонд департамента
        /// </summary>
        public int SalaryFund { get; set; }
        #endregion

        /// <summary>
        /// Конструктов экземпляра класса Department
        /// </summary>
        /// <param name="name">Наименование департамента</param>
        public Department (string name)
        {
            ///Присвоение Id департамента
            if (AllDepartmentsId != null)
            {
                Id = AllDepartmentsId.Max() + 1;
            }
            else
            {
                Department.AllDepartmentsId = new List<int>();
                Id = 1; //важно, Id = 0 резерв для сотрудников, не подчиненных никаким департаментам, например, директора
                //и для высших департаментов, которые никому не подчинены
            }
            Department.AllDepartmentsId.Add(Id); //добавляем Id департамента в статическую коллекцию
            SlaveDepartmentId = new List<int>(0); //важно! инициализируем с вместимость 0, на этом основан расчет ЗП руководителей
            HigherDepartmentId = 0; //для начала вышестоящий департамент присваиваем 0
            DepartmentEmployeesId = new List<int>(); //инициализируем коллекцию работников
            SalaryCalculatedFlag = false; //ставим флаг, что зарплатный фонд не рассчитан
            Name = name; //присваиваем наименование департамента
        }

        /// <summary>
        /// Очистка статической коллекции Id всех департаментов
        /// </summary>
        public static void ClearIds()
        {
            AllDepartmentsId = null;
        }

        
    }
}
