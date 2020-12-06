using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Homework_12_ver_1
{
    class TopManager : Worker
    {
        #region Поля и свойства класса
        /// <summary>
        /// Имя класса экземпляра
        /// </summary>
        public override string ClassType { get; }

        /// <summary>
        /// Поле зарплата
        /// </summary>
        private int salary;

        /// <summary>
        /// Свойство зарплата (10 % от переданного значения (прибыли))
        /// </summary>
        public override int Salary { get { return salary; } set { salary = value * 10 / 100; } }
        #endregion

        /// <summary>
        /// Конструктор только для Json (приватный)
        /// </summary>
        [JsonConstructor] //ВАЖНО! Отметка для JsonConstructor использовать этот конструктор при десериализации
        private TopManager(int id, string name, string surname, int age, string position, int departmentId, string deprtmentname, 
            int projects, int salary, string classtype)
           : base(id, name, surname, age, position, departmentId, deprtmentname, projects)
        {
            Salary = salary;
            ClassType = classtype;
        }

        /// <summary>
        /// Конструктор TopManager, наследован от конструктора базового класса Worker
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="projects">Проекты</param>
        //[JsonConstructor] //ВАЖНО! Отметка для JsonConstructor использовать этот конструктор при десериализации
        public TopManager(string name, string surname, int age, string position, int projects)
            : base(name, surname, age, position, projects)
        {
            ClassType = "Высший менеджмент";
            DepartmentId = 0;
            DepartmentName = "";
        }

        /// <summary>
        /// Конструктор создания экземпляра TopManager на основе имеющегося экземпляра класса наследника Worker (используется при изменении
        /// типа класса-наследника Worker)
        /// </summary>
        /// <param name="worker">Исходный экземпляра класса-наследника Worker</param>
        public TopManager(Worker worker) :
             base(worker)
        {
            ClassType = "Высший менеджмент";
            DepartmentId = 0;
            DepartmentName = "";
        }


    }
}
