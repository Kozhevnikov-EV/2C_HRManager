using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_11
{
    class TopManager : Worker
    {
        /// <summary>
        /// Поле зарплата
        /// </summary>
        private int salary;

        /// <summary>
        /// Свойство зарплата (10 % от переданного значения (прибыли))
        /// </summary>
        public override int Salary { get { return salary; } set { salary = value * 10 / 100; } }

        /// <summary>
        /// Конструктор TopManager, наследован от конструктора базового класса Worker
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="projects">Проекты</param>
        public TopManager(string name, string surname, int age, string position, int projects)
            : base(name, surname, age, position, projects)
        {
            
        }


    }
}
