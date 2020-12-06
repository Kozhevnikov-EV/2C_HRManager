using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Homework_12_ver_1
{
    class Intern : Worker
    {
        #region Свойства класса
        /// <summary>
        /// Зарплата
        /// </summary>
        public override int Salary { get; set; }

        /// <summary>
        /// Имя класса экземпляра
        /// </summary>
        public override string ClassType { get; }
        #endregion

        /// <summary>
        /// Конструктор для JsonConverter (приватный)
        /// </summary>
        [JsonConstructor] //ВАЖНО! Отметка для JsonConstructor использовать этот конструктор при десериализации
        private Intern(int id, string name, string surname, int age, string position, int departmentId, string deprtmentname, int projects,
            int salary, string classtype)
           : base(id, name, surname, age, position, departmentId, deprtmentname, projects)
        {
            Salary = salary;
            ClassType = classtype;
        }

        /// <summary>
        /// Основной конструктор Intern, наследован от конструктора Worker
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="departmentId">Id департамента</param>
        /// <param name="projects">Проекты</param>
        public Intern(string name, string surname, int age, string position, int departmentId, int projects)
            : base(name, surname, age, position, departmentId, projects)
        {
            this.Salary = 500; //зп по умолчанию
            ClassType = "Студент";
        }

        /// <summary>
        /// Конструктор Intern, используется при копировании экземпляра класса-наследника Worker в экземпляр другого класса-наследника Worker
        /// Например Worker=>Intern
        /// </summary>
        /// <param name="worker">Исходный экземпляр класса-наследника для копирования</param>
        public Intern(Worker worker) :
            base (worker)
        {
            Salary = 500; //зп по умолчанию
            ClassType = "Студент";
        }

    }
}
