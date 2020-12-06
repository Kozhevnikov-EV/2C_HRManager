using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_11
{
    class Workman : Worker
    {
        /// <summary>
        /// Поле зарплата
        /// </summary>
        private int salary;

        /// <summary>
        /// Зарплата (получается умножением количества рабочих часов на часовую ставку)
        /// </summary>
        public override int Salary { get { return salary; } set { salary = WageRate * WorkHours; } }

        /// <summary>
        /// Часовая тарифная ставка
        /// </summary>
        public int WageRate { get; set; }
        
        /// <summary>
        /// Количество рабочих часов в месяце
        /// </summary>
        public int WorkHours { get; set; }

        /// <summary>
        /// Конструктор Workman, наследован от Worker
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="departmentId">Id департамента</param>
        /// <param name="projects">Проекты</param>
        public Workman(string name, string surname, int age, string position, int departmentId, int projects)
            : base(name, surname, age, position, departmentId, projects)
        {
            WageRate = 12; //часовая тарифная ставка фиксированная для сотрудников
            WorkHours = 168; //среднее количество рабочих часов в месяце
            Salary = 0; //присваиваем любое значение, чтобы был инициализирован расчет через мутатор
        }
    }
}
