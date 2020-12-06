using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Homework_12_ver_1
{
    class Workman : Worker
    {
        #region Поля и свойства
        /// <summary>
        /// Имя класса экземпляра
        /// </summary>
        public override string ClassType { get; }

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
        #endregion

        /// <summary>
        /// Конструктор только для Json (приватный)
        /// </summary>
        [JsonConstructor] //ВАЖНО! Отметка для JsonConstructor использовать этот конструктор при десериализации
        private Workman(int id, string name, string surname, int age, string position, int departmentId, string deprtmentname, int projects,
            int wagerate, int workhours, int salary, string classtype)
           : base(id, name, surname, age, position, departmentId, deprtmentname, projects)
        {
            WageRate = wagerate;
            WorkHours = workhours;
            Salary = salary;
            ClassType = classtype;
        }

        /// <summary>
        /// Основной конструктор Workman, наследован от Worker
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
            ClassType = "Штатный сотрудник";
        }

        /// <summary>
        /// Конструктор создания экземпляра Workman на основе имеющегося экземпляра класса наследника Worker (используется при изменении
        /// типа класса-наследника Worker)
        /// </summary>
        /// <param name="worker">Исходный экземпляра класса-наследника Worker</param>
        public Workman(Worker worker) :
            base(worker)
        {
            WageRate = 12; //часовая тарифная ставка фиксированная для сотрудников
            WorkHours = 168; //среднее количество рабочих часов в месяце
            Salary = 0; //присваиваем любое значение, чтобы был инициализирован расчет через мутатор
            ClassType = "Штатный сотрудник";
        }
    }
}
