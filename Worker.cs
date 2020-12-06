using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace Homework_11
{
    abstract class Worker
    {
        /// <summary>
        /// Зарплата
        /// </summary>
        public abstract int Salary { get; set; }

        /// <summary>
        /// ID Сотрудника
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Статичный список Id
        /// </summary>
        static private List<int> AllWorkerIds { get; set; }
        
        /// <summary>
        /// Имя сотрудника
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Фамилия сотрудника
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Возраст сотрудника
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Id Департамента
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Название департамента
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Количество проектов
        /// </summary>
        public int Projects { get; set; }

        

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="departmentId">Id Департамента</param>
        /// <param name="projects">Проекты</param>
        public Worker(string name, string surname, int age, string position, int departmentId, int projects)
        {
            if (Worker.AllWorkerIds != null) //если уже существуют экземпляры данного класса
            {
                Id = AllWorkerIds.Max() + 1; //находим максимальное значение и увеличиваем на единицу
            }
            else //если первый экземпляр данного класса
            {
                Worker.AllWorkerIds = new List<int>(); //инициализируем коллекцию
                Id = 1; //единица, 0 оставляем для резерва на будущее
            }
            Worker.AllWorkerIds.Add(Id); //добавляем Id в статическую коллекцию Id данного класса
            Name = name;
            Surname = surname;
            Age = age;
            Position = position;
            DepartmentId = departmentId;
            Projects = projects;            
        }

        /// <summary>
        /// Конструктор, используемый для класса-потомка TopManager
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="age">Возраст</param>
        /// <param name="position">Должность</param>
        /// <param name="projects">Проекты</param>
        public Worker(string name, string surname, int age, string position, int projects)
            : this (name, surname, age, position, 0, projects)
        {

        }

        /// <summary>
        /// Конструктор, инициализирующий пустой экземпляр класса с присваиванием ему Id в статичной коллекции
        /// </summary>
        public Worker()
        {
            if (Worker.AllWorkerIds != null)
            {
                Id = AllWorkerIds.Max() + 1;
            }
            else
            {
                Worker.AllWorkerIds = new List<int>();
                Id = 1;
            }
            Worker.AllWorkerIds.Add(Id);
        }

        /// <summary>
        /// Метод очистки статичной коллекции Id
        /// </summary>
        public static void ClearIds()
        {
            AllWorkerIds = null;
        }
    }
}

