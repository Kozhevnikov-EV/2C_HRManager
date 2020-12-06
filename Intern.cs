using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_11
{
    class Intern : Worker
    {
        /// <summary>
        /// Зарплата
        /// </summary>
        public override int Salary { get; set; }

        /// <summary>
        /// Конструктор Intern, наследован от конструктора Worker
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
        }
    }
}
