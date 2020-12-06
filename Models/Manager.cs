using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Homework_12_ver_1
{
    class Manager : Worker
    {
        #region Поля и свойства класса
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
        /// Имя класса экземпляра
        /// </summary>
        public override string ClassType { get; }
        #endregion

        /// <summary>
        /// Конструктор только для Json (приватный)
        /// </summary>
        [JsonConstructor] //ВАЖНО! Отметка для JsonConstructor использовать этот конструктор при десериализации
        private Manager(int id, string name, string surname, int age, string position, int departmentId, string deprtmentname, int projects,
            int salary, string classtype)
           : base(id, name, surname, age, position, departmentId, deprtmentname, projects)
        {
            Salary = salary;
            ClassType = classtype;
        }

        /// <summary>
        /// Конструктор Manager, наследован от конструктора Worker
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="departmentId">Id департамента</param>
        /// <param name="projects">Проекты</param>
        //[JsonConstructor] //ВАЖНО! Отметка для JsonConstructor использовать этот конструктор при десериализации
        public Manager(string name, string surname, int age, string position, int departmentId, int projects)
            : base(name, surname, age, position, departmentId, projects)
        {
            Salary = 0; //присваиваем любое значение, чтобы был инициализирован расчет через мутатор
            ClassType = "Руководитель департамента";
        }

        /// <summary>
        /// Конструктор создания экземпляра Manager на основе имеющегося экземпляра класса наследника Worker (используется при изменении
        /// типа класса-наследника Worker)
        /// </summary>
        /// <param name="worker">Исходный экземпляра класса-наследника Worker</param>
        public Manager(Worker worker) :
            base(worker)
        {
            Salary = 0; //присваиваем любое значение, чтобы был инициализирован расчет через мутатор
            ClassType = "Руководитель департамента";
        }
    }
}
