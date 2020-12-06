using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Перечисление критериев сортировки
    /// </summary>
    enum SortedCriterion
    {
        Id,
        IdDescending,
        Name,
        NameDescending,
        Surname,
        SurnameDescending,
        Age,
        AgeDescending,
        Position,
        PositionDescending,
        Salary,
        SalaryDescending,
        Projects,
        ProjectsDescending
    }

    abstract class Worker
    {
        #region Свойства класса
        /// <summary>
        /// Зарплата
        /// </summary>
        public abstract int Salary { get; set; }

        /// <summary>
        /// Имя класса экземпляра
        /// </summary>
        public abstract string ClassType { get; }

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
        #endregion

        /// <summary>
        /// Базовый конструктор, используется для Json
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="age"></param>
        /// <param name="position"></param>
        /// <param name="departmentId"></param>
        /// <param name="departmentname"></param>
        /// <param name="projects"></param>
        [JsonConstructor]
        public Worker(int id, string name, string surname, int age, string position, int departmentId, string departmentname, int projects)
        {
            Id = id;
            AllWorkerIds = new List<int>();
            Worker.AllWorkerIds.Add(Id);
            Name = name;
            Surname = surname;
            Age = age;
            Position = position;
            DepartmentId = departmentId;
            DepartmentName = departmentname;
            Projects = projects;
        }

        /// <summary>
        /// Основной базовый конструктор для создания экземпляра класса-наследника
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
            : this(name, surname, age, position, 0, projects)
        { }

        /// <summary>
        /// Конструктор для копирования экземпляра работника (изменение типа наследованного класса, например Intern => Workman)
        /// </summary>
        /// <param name="worker">Экземпляр исходного класса</param>
        public Worker(Worker worker)
        {
            Id = worker.Id;
            Name = worker.Name;
            Surname = worker.Surname;
            Age = worker.Age;
            Position = worker.Position;
            Projects = worker.Projects;
            DepartmentId = worker.DepartmentId;
            DepartmentName = worker.DepartmentName;
        }

        /// <summary>
        /// Метод очистки статичной коллекции Id
        /// </summary>
        public static void ClearIds()
        {
            AllWorkerIds = null;
        }

        #region Метод сортировки и вложенные классы с сортировкой компаратором

        /// <summary>
        /// Компаратор с выбором метода сортировки по критерию
        /// </summary>
        /// <param name="Criterion">Критерий сортировки</param>
        /// <returns></returns>
        public static IComparer<Worker> SortedBy(SortedCriterion Criterion)
        {
            if (Criterion == SortedCriterion.Id) return new SortById();
            else if (Criterion == SortedCriterion.IdDescending) return new SortByIdDescending();
            else if (Criterion == SortedCriterion.Name) return new SortByName();
            else if (Criterion == SortedCriterion.NameDescending) return new SortByNameDescending();
            else if (Criterion == SortedCriterion.Surname) return new SortBySurname();
            else if (Criterion == SortedCriterion.SurnameDescending) return new SortBySurnameDescending();
            else if (Criterion == SortedCriterion.Age) return new SortByAge();
            else if (Criterion == SortedCriterion.AgeDescending) return new SortByAgeDescending();
            else if (Criterion == SortedCriterion.Position) return new SortByPosition();
            else if (Criterion == SortedCriterion.PositionDescending) return new SortByPositionDescending();
            else if (Criterion == SortedCriterion.Salary) return new SortBySalary();
            else if (Criterion == SortedCriterion.SalaryDescending) return new SortBySalaryDescending();
            else if (Criterion == SortedCriterion.Projects) return new SortByProjects();
            else return new SortByProjectsDescending();
        }

        private class SortById : IComparer<Worker>
        { 
            public int Compare(Worker x, Worker y)
            {
                    Worker X = (Worker)x;
                    Worker Y = (Worker)y;
                    if (X.Id == Y.Id) return 0;
                    else if (X.Id > Y.Id) return 1;
                    else return -1;
            }
        }

        private class SortByIdDescending : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                if (Y.Id == X.Id) return 0;
                else if (Y.Id > X.Id) return 1;
                else return -1;
            }
        }

        private class SortByName : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                return String.Compare(X.Name, Y.Name);
            }
        }

        private class SortByNameDescending : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                return String.Compare(Y.Name, X.Name);
            }
        }

        private class SortBySurname : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                return String.Compare(X.Surname, Y.Surname);
            }
        }

        private class SortBySurnameDescending : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                return String.Compare(Y.Surname, X.Surname);
            }
        }

        private class SortByAge : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                if (X.Age == Y.Age) return 0;
                else if (X.Age > Y.Age) return 1;
                else return -1;
            }
        }

        private class SortByAgeDescending : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                if (Y.Age == X.Age) return 0;
                else if (Y.Age > X.Age) return 1;
                else return -1;
            }
        }

        private class SortByPosition : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                return String.Compare(X.Position, Y.Position);
            }
        }

        private class SortByPositionDescending : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                return String.Compare(Y.Position, X.Position);
            }
        }

        private class SortBySalary : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                if (X.Salary == Y.Salary) return 0;
                else if (X.Salary > Y.Salary) return 1;
                else return -1;
            }
        }

        private class SortBySalaryDescending : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                if (Y.Salary == X.Salary) return 0;
                else if (Y.Salary > X.Salary) return 1;
                else return -1;
            }
        }

        private class SortByProjects : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                if (X.Projects == Y.Projects) return 0;
                else if (X.Projects > Y.Projects) return 1;
                else return -1;
            }
        }

        private class SortByProjectsDescending : IComparer<Worker>
        {
            public int Compare(Worker x, Worker y)
            {
                Worker X = (Worker)x;
                Worker Y = (Worker)y;
                if (Y.Projects == X.Projects) return 0;
                else if (Y.Projects > X.Projects) return 1;
                else return -1;
            }
        }
        #endregion
    }
}

