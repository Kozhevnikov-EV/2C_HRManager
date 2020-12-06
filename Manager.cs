using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_11
{
    class Manager : Worker
    {
        /// <summary>
        /// Поле процент (для расчета ЗП руководителя)
        /// </summary>
        private static int procent = 15;

        /// <summary>
        /// Свойство процент (для расчета ЗП руководителя)
        /// </summary>
        public static int Procent { get { return procent; } }

        /// <summary>
        /// Поле зарплата
        /// </summary>
        private int salary;

        /// <summary>
        /// Свойство зарплата (не менее 1300)
        /// </summary>
        public override int Salary { get { return salary; } set { if (value < 1300) { salary = 1300; } else { salary = value; } } }

        /// <summary>
        /// Конструктор Manager, наследован от конструктора Worker
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="departmentId">Id департамента</param>
        /// <param name="projects">Проекты</param>
        public Manager(string name, string surname, int age, string position, int departmentId, int projects)
            : base(name, surname, age, position, departmentId, projects)
        {
            Salary = 0; //присваиваем любое значение, чтобы был инициализирован расчет через мутатор
        }
    }
}
