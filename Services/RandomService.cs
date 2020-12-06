using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Статический класс для создания демонстрационной организации
    /// </summary>
    static class RandomService
    {
        /// <summary>
        /// Статичный генератор случайных чисел
        /// </summary>
        static private Random R = new Random();

        /// <summary>
        /// Основной метод генерации демонстрационной организации
        /// </summary>
        /// <param name="organization">Экземпляр организации</param>
        /// <param name="Name">Имя организации</param>
        public static void RandomOrganization(Organization organization, string Name)
        {
            Worker.ClearIds(); //чистим Id работников
            Department.ClearIds(); //чистим Id департаментов
            organization.Workers = new ObservableCollection<Worker>(); //инициализируем коллекцию работников в экземпляре класса
            organization.Departments = new ObservableCollection<Department>(); //аналогично инициализируем коллекцию департамента
            organization.TopManagersId = new ObservableCollection<int>();//инициализируем коллекцию топ-топ менеджеров
            organization.UnallocatedWorkersId = new ObservableCollection<int>();//инициализируем коллекцию нераспределенных сотрудников
            organization.SlaveDepartmentId = new ObservableCollection<int>(); //инициализируем коллекцию подчиненных департаментов
            //добавляем в него департамент из метода создания демострационной организации
            organization.SlaveDepartmentId.Add(RandomDepartment(organization, "Департамент 1", 0, 3, 3)); 
            SalaryService.MainMethodSetSalaryForManagers(organization); //Рассчитываем ЗП руководителей и зарплатные фонды департаментов
            organization.Name = Name; //присваиваем организации переданное в метод имя
            organization.Profit = R.Next(1, 101) * 1000000; //генерируем случайную прибыль
            TopManager Director = NoRandomDirector(organization.Profit); //создаем директора и передаем значение прибыли для расчета его ЗП
            organization.TopManagersId.Add(Director.Id); //Собственно, прописываем Id созданного директора в соответствующее поле организации
            organization.Workers.Add(Director); //добавляем директора в список работников
        }

        /// <summary>
        /// Создание демострационных департаментов (внимание, рекурсивный метод)
        /// </summary>
        /// <param name="organization">Экземпляр организации</param>
        /// <param name="Name">Название основного департамента</param>
        /// <param name="HigherDepartmentId">Id вышестоящего департамента (данный параметр ставим 0!) Нужен для рекурсии</param>
        /// <param name="Levels">Число уровней подчиненности департаментов</param>
        /// <param name="Parallels">Количество департаментов одного уровня</param>
        /// <returns>Id департамента первого уровня</returns>
        private static int RandomDepartment(Organization organization, string Name, int HigherDepartmentId, int Levels, int Parallels)
        {
            if (Levels > 0)
            {
                Department NewDepartment = new Department(Name);// = new Department(); //создаем экземпляр департамента
                                                                //Далее в цикле добавляем в департамент начальника, сотрудников и студентов
                organization.Workers.Add(RandomManager(NewDepartment)); //Добавляем руководителя со случайными полями и делаем связку по Id
                NewDepartment.HigherDepartmentId = HigherDepartmentId;
                for (int j = 0; j < R.Next(1, 11); j++) //создаем случайное количество сотрудников
                {
                    organization.Workers.Add(RandomWorkman(NewDepartment)); //Добавляем сотрудника со случайными полями и связываем по Id
                }

                for (int k = 0; k < R.Next(1, 4); k++) //создаем случайное количество студентов
                {
                    organization.Workers.Add(RandomIntern(NewDepartment)); ////Добавляем интерна со случайными полями и связываем по Id
                }
                Levels--; //т.к. один уровень депаратментов создан, то уменьшаем Levels на 1
                if (Levels > 0)
                {
                    for (int i = 1; i <= Parallels; i++) //далее в цикле создаем необходимое число департаментов в одном уровне подчиненности
                    {
                        NewDepartment.SlaveDepartmentId.Add(RandomDepartment(organization, Name + i, NewDepartment.Id, Levels, Parallels)); //рекурсия!!!
                    }
                }
                organization.Departments.Add(NewDepartment); ///добавляем созданный департамент в коллекцию департаментов
                return NewDepartment.Id; //возвращаем Id созданного департамента
            }
            return Levels; //ну что-то надо вернуть по окончании рекурсии, иначе компилятор недоволен
        }

        /// <summary>
        /// Метод создания неслучайного директора
        /// </summary>
        /// <param name="Profit">Прибыль организации</param>
        /// <returns>Экземпляр TopManager</returns>
        private static TopManager NoRandomDirector(int Profit)
        {
            TopManager NoRndTopManager = new TopManager("Сова", "Лесная", R.Next(20, 41), "Директор", R.Next(10, 20)); //создаем директора
            NoRndTopManager.Salary = Profit; //передаем ему всю прибыль
            return NoRndTopManager; //возвращаем директора
        }

        /// <summary>
        /// Создание экземпляра руководителя со случайными значениями полей
        /// </summary>
        /// <param name="NewDepartment">Департамент, в котором будет числиться руководитель</param>
        /// <returns>Экземпляр Manager</returns>
        private static Manager RandomManager(Department NewDepartment)
        {
            Manager RndManager = new Manager(
                $"Имя_{R.Next(1, 99)}", $"Фамилия_{R.Next(1, 99)}", R.Next(22, 71), "Руководитель", NewDepartment.Id, R.Next(1, 8));
            //создаем руководителя
            NewDepartment.ChiefId = RndManager.Id; //записываем Id руководителя в департамент
            RndManager.DepartmentName = NewDepartment.Name; //указываем у руководителя название департамента
            return RndManager; //возвращаем экземпляр руководителя
        }

        /// <summary>
        /// Создание экземпляра сотрудника со случайными полями
        /// </summary>
        /// <param name="NewDepartment">Департамент, в котором будет числиться сотрудник</param>
        /// <returns>Экземпляр Workman</returns>
        private static Workman RandomWorkman(Department NewDepartment)
        {
            Workman RndWorkman = new Workman(
                $"Имя_{R.Next(1, 99)}", $"Фамилия_{R.Next(1, 99)}", R.Next(18, 71), "Сотрудник", NewDepartment.Id, R.Next(1, 6));
            //создаем сотрудника
            RndWorkman.DepartmentName = NewDepartment.Name; //прописываем ему поле "название департамента"
            NewDepartment.DepartmentEmployeesId.Add(RndWorkman.Id); //добавляем Id сотрудника в список Id работников департамента
            return RndWorkman; //возвращаем экземпляр сотрудника
        }

        /// <summary>
        /// Создание экземпляра интерна со случайными полями
        /// </summary>
        /// <param name="NewDepartment">Департамент, в котором будет числиться интерн</param>
        /// <returns>Экземпряр Intern</returns>
        private static Intern RandomIntern(Department NewDepartment)
        {
            Intern RndIntern = new Intern(
                        $"Имя_{R.Next(1, 99)}", $"Фамилия_{R.Next(1, 99)}", R.Next(18, 40), "Практикант", NewDepartment.Id, R.Next(1, 4));
            //создаем интерна
            RndIntern.DepartmentName = NewDepartment.Name; //прописываем ему поле "название департамента"
            NewDepartment.DepartmentEmployeesId.Add(RndIntern.Id); //добавляем Id интерна в список Id работников департамента
            return RndIntern; //возвращаем экземпляр интерна
        }
    }
}
