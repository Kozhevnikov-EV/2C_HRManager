using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Documents;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Homework_11
{
    class Organization
    {
        #region Поля и свойства класса
        /// <summary>
        /// Статичный генератор случайных значений
        /// </summary>
        static private Random R = new Random();

        /// <summary>
        /// Название организации
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id директора
        /// </summary>
        public int DirectorId { get; set; }

        /// <summary>
        /// Список Id подчиненных департаментов
        /// </summary>
        public ObservableCollection<int> SlaveDepartmentId { get; set; }

        /// <summary>
        /// Прибыль организации
        /// </summary>
        public int Profit { get; set; }
        
        /// <summary>
        /// Коллекция всех департаментов организации
        /// </summary>
        public ObservableCollection<Department> Departmets { get; set; }

        /// <summary>
        /// Коллекция всех работников организации
        /// </summary>
        public ObservableCollection<Worker> Workers { get; set; }
        #endregion

        /// <summary>
        /// Конструктор создания демонстрационной организации
        /// </summary>
        /// <param name="name">Имя организации</param>
        public Organization (string name)
        {
            Worker.ClearIds(); //чистим Id работников
            Department.ClearIds(); //чистим Id департаментов
            Workers = new ObservableCollection<Worker>(); //инициализируем коллекцию работников в экземпляре класса
            Departmets = new ObservableCollection<Department>(); //аналогично инициализируем коллекцию департамента
            SlaveDepartmentId = new ObservableCollection<int>(); //инициализируем коллекцию подчиненных департаментов
            SlaveDepartmentId.Add(RandomDepartment("Департамент 1", 0, 5, 3)); //добавляем в него департамент из метода создания демострационной
                                                                               //организации
            MainMethodSetSalaryForManagers(); //Рассчитываем зарплаты руководителей и зарплатные фонды департаментов
            Name = name; //присваиваем организации переданное в метод имя
            Profit = R.Next(1, 101) * 1000000; //генерируем случайную прибыль
            TopManager Director = NoRandomDirector(Profit); //создаем директора и передаем значение прибыли для расчета его ЗП
            this.DirectorId = Director.Id; //Собственно, прописываем Id созданного директора в соответствующее поле организации
            Workers.Add(Director); //добавляем директора в список работников
            
        }

        #region Методы сериализации/десериализации json
        /// <summary>
        /// Метод создания экземпляра организации из json файла
        /// </summary>
        /// <param name="Path">Путь к файлу</param>
        /// <returns>Экземпляр организации десериализованный из указанного файла</returns>
        public static Organization LoadFromJson(string Path)
        {
            Worker.ClearIds(); //чистим Id работников
            Department.ClearIds(); //чистим Id департаментов
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }; //в переменную с настройками для Json конвертера
            //прописываем запись всех .NET типов имен (чтобы конвертер знал какой объект он конвертирует)
            var json = File.ReadAllText(Path); //считываем файл в текстовую переменную
            try
            {
                Organization repository = JsonConvert.DeserializeObject<Organization>(json, settings); //десериализуем
                return repository; //возвращаем десериализованный экземпляр
            }
            catch (Exception Ex) //обрабатываем исключение если файл поврежден
            {
                MessageBox.Show(Ex.Message, "А base.json то, ненастоящий!", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                Organization Nullrepository = null;
                return Nullrepository; //надо что-то вернуть, иначе компилятор ругается ¯\_(ツ)_/¯
            }

        }

        /// <summary>
        /// Сохранение экземпляра организации в файл json
        /// </summary>
        /// <param name="repository">Экземпляр организации для сериализации</param>
        /// <param name="Path">Путь сохранения файла</param>
        public static void SaveToJson(Organization repository, string Path)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All}; //в переменную с настройками для Json конвертера
            //прописываем запись всех .NET типов имен (чтобы конвертер знал какой объект он конвертирует)
            var json = JsonConvert.SerializeObject(repository, settings); //сериализуем
            File.WriteAllText(Path, json); //пишем в файл
        }
        #endregion

        #region Методы создания случайных департаментов, руководителей, сотрудников и интернов
        /// <summary>
        /// Создание демострационных департаментов (внимание, рекурсивный метод)
        /// </summary>
        /// <param name="Name">Название основного департамента</param>
        /// <param name="HigherDepartmentId">Id вышестоящего департамента (данный параметр ставим 0!) Нужен для рекурсии</param>
        /// <param name="Levels">Число уровней подчиненности департаментов</param>
        /// <param name="Parallels">Количество департаментов одного уровня</param>
        /// <returns>Id департамента первого уровня</returns>
        private int RandomDepartment (string Name, int HigherDepartmentId, int Levels, int Parallels)
        {
            if (Levels > 0)
            {
                Department NewDepartment = new Department(Name);// = new Department(); //создаем экземпляр департамента
                                                               //Далее в цикле добавляем в департамент начальника, сотрудников и студентов
                Workers.Add(RandomManager(NewDepartment)); //Добавляем руководителя со случайными полями и делаем связку по Id

                for (int j = 0; j < R.Next(1, 11); j++) //создаем случайное количество сотрудников
                {
                    Workers.Add(RandomWorkman(NewDepartment)); //Добавляем сотрудника со случайными полями и связываем по Id
                }

                for (int k = 0; k < R.Next(1, 4); k++) //создаем случайное количество студентов
                {
                    Workers.Add(RandomIntern(NewDepartment)); ////Добавляем интерна со случайными полями и связываем по Id
                }
                Levels--; //т.к. один уровень депаратментов создан, то уменьшаем Levels на 1
                for (int i = 1; i <= Parallels; i++) //далее в цикле создаем необходимое число департаментов в одном уровне подчиненности
                {
                    NewDepartment.SlaveDepartmentId.Add(RandomDepartment(Name + i, NewDepartment.Id, Levels, Parallels)); //рекурсия!!!
                }
                Departmets.Add(NewDepartment); ///добавляем созданный департамент в коллекцию департаментов
                return NewDepartment.Id; //возвращаем Id созданного департамента
            }
            return Levels; //ну что-то надо вернуть по окончании рекурсии, иначе компилятор недоволен
        }

        /// <summary>
        /// Метод создания неслучайного директора
        /// </summary>
        /// <param name="Profit">Прибыль организации</param>
        /// <returns>Экземпляр TopManager</returns>
        private static TopManager NoRandomDirector (int Profit)
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
                        $"Имя_{R.Next(1, 99)}", $"Фамилия_{R.Next(1, 99)}", R.Next(18, 40), "Студентик", NewDepartment.Id, R.Next(1, 4));
            //создаем интерна
            RndIntern.DepartmentName = NewDepartment.Name; //прописываем ему поле "название департамента"
            NewDepartment.DepartmentEmployeesId.Add(RndIntern.Id); //добавляем Id интерна в список Id работников департамента
            return RndIntern; //возвращаем экземпляр интерна
        }
        #endregion

        #region Методы рассчета зарплатных фондов департаментов и ЗП руководителей департаментов
        /// <summary>
        /// Основной метод расчета ЗП руководителей и зарплатного фонда департаментов
        /// </summary>
        private void MainMethodSetSalaryForManagers()
        {
            SetLowerManagersSalary(); //метод расчета ЗП руководителей и зарплатного фонда департаментов низших уровней (без подчиненных департаментов)
            SetHigherManagersSalary();  //метод расчета ЗП вышестоящих руководителей и зарплатного фонда вышестоящих департаментов
        }

        /// <summary>
        /// Расчет ЗП руководителей и зарплатных фондов департаментов низших уровней (без подчиненных сотрудников)
        /// </summary>
        private void SetLowerManagersSalary()
        {
            int Id; //переменная для хранения Id текущего руководителя
            int SumOfSalary = 0; //сюда будем записывать текущее значение ЗП;
            for (int i = 0; i < Departmets.Count; i++) //цикл, перебирающий департаменты в коллекции департаментов
            {
                if (Departmets[i].SlaveDepartmentId.Count == 0) //если у департамента нет других департаментов в подчинении
                {
                    Id = Departmets[i].ChiefId; //в переменную записываем значение Id руководителя этого департамента
                    SumOfSalary += FindEmployeesSalary(Departmets[i].Id); // считаем сумму ЗП работников данного департамента
                    foreach (var worker in Workers) //цикл, перебирающий работникв в коллекции работников
                    {
                        if (worker.Id == Id) //находим Начальника департамента
                        {
                            worker.Salary = SumOfSalary * Manager.Procent / 100; //расчитываем ему ЗП и записываем в соответствующее поле
                            Departmets[i].SalaryFund += SumOfSalary + worker.Salary; //рассчитываем общий зарплатный фонд департамента
                            Departmets[i].SalaryCalculatedFlag = true; //Делаем отметку в департаменте, что ЗП расчитана
                            SumOfSalary = 0; //обнуляем переменную, хранящую текущее значение ЗП
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Расчет ЗП руководителей и зарплатных фондов вышестоящих департаментов
        /// </summary>
        private void SetHigherManagersSalary()
        {
            while (!AllSalaryIsKnown()) //метод выполняется до тех пор, пока не рассчитаем все зарплатные фонды всех департаментов
            {
                int SumOfSalary = 0; //сюда будем записывать текущее значение ЗП;
                for (int i = 0; i < Departmets.Count; i++) //в цикле перебираем все департаменты в коллекции департаментов
                {
                    //Если ЗП фонд департамента не расчитан, а ЗП фонд подчиненных ему департаментов рассчитан, то
                    if (Departmets[i].SalaryCalculatedFlag == false && ReadyForSalaryCalculation(Departmets[i].SlaveDepartmentId) == true)
                    {
                        //суммируем зарплатные фонды нижестоящих подчиненных департаментов
                        SumOfSalary += SalaryFundLowerDepartments(Departmets[i].SlaveDepartmentId);
                        //добавляем сумму ЗП сотрудников департаментов
                        SumOfSalary += FindEmployeesSalary(Departmets[i].Id);
                        foreach (var worker in Workers) //перебираем всех работников в коллекции работников
                        {
                            if (worker.Id == Departmets[i].ChiefId) //находим начальника департамента
                            {
                                worker.Salary = SumOfSalary * Manager.Procent / 100; //считаем ЗП начальника
                                Departmets[i].SalaryFund = FindEmployeesSalary(Departmets[i].Id) + worker.Salary; //считаем ЗП фонд департамента
                                Departmets[i].SalaryCalculatedFlag = true; //Делаем отметку в департаменте, что ЗП расчитана
                                SumOfSalary = 0;
                            } //обнуляем переменную, хранящую текущее значение ЗП
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Проверка полноты расчета зарплатных фондов всех департаментов
        /// </summary>
        /// <returns>true - во всех департаментов рассчитаны зарплатные фонды и ЗП руководителей</returns>
        private bool AllSalaryIsKnown()
        {
            bool Answer = false; //переменная, значение которой будем возвращать
            foreach (var dep in Departmets) //перебираем департаменты в коллекции департаментов
            {
                if(dep.SalaryCalculatedFlag == true) //если ЗП расчитана, то Answer присваиваем true
                {
                    Answer = true;
                }
                else
                {
                    Answer = false; //если хоть в одном департаменте ЗП не расчитана, то прерываем цикл и возвращаем false
                    break;
                }
            }
            return Answer;
        }

        /// <summary>
        /// Расчет суммы зарплатных фондов всех нижестоящих департаментов (рекурсивный метод!)
        /// </summary>
        /// <param name="DepartmentsIds">Коллекция Id департаментов прямой подчиненности</param>
        /// <returns>Сумму зарплатных фондов всех нижестоящих департаментов</returns>
        private int SalaryFundLowerDepartments(List<int> DepartmentsIds)
        {
            int SalaryFund = 0; //переменная, куда будем суммировать зарплатные фонды
            foreach (var department in Departmets) //перебираем в цикле все департаменты в коллекции департаментов
            {
                if (DepartmentsIds.Contains(department.Id)) //если Id департамента есть в списке прямой подчиненности старшего департамента
                {
                    SalaryFund += department.SalaryFund; //то добавляем его зарплатный фонд в нашу переменную
                    if (department.SlaveDepartmentId.Count != 0) //если у этого департамента также есть в подчинении нижестоящие департаменты
                    {
                        SalaryFund += SalaryFundLowerDepartments(department.SlaveDepartmentId); //то используем тот же метод для поисках их 
                        //запрплатных фондов (рекурсия!)
                    }    
                }
            }
            return SalaryFund; //возвращаем сумму зарплатных фондов всех нижестоящих департаментов
        }

        /// <summary>
        /// Проверка наличия рассчитанного зарплатного фонда в подчиненных департаментах
        /// </summary>
        /// <param name="SlaveDepartmentId">Коллекция Id под подчиненных департаментов</param>
        /// <returns>true - в подчиненных департаментах зарплатные фонды расчитаны</returns>
        private bool ReadyForSalaryCalculation(List<int> SlaveDepartmentId)
        {
            bool Ready = false; //переменная для возврата из метода
            for (int i = 0; i < Departmets.Count; i++) //перебираем все департаменты в коллекции департаментов
            {
                //если в любом из подчиненных департаментов зарплатный фонд не рассчитан
                if (SlaveDepartmentId.Contains(Departmets[i].Id) && Departmets[i].SalaryCalculatedFlag == false)
                {
                    Ready = false; //то в переменную с ответом пишем false
                    break; //прерываем цикл
                }
                else
                {
                    Ready = true;
                }
            }
            return Ready; //возращаем ответ
        }



        /// <summary>
        /// Метод калькуляции суммы зарплаты работников департамента (кроме руководителя)
        /// </summary>
        /// <param name="DepartmentId">Id департамента</param>
        /// <returns>Возвращает сумму зарплат работников департамента (за исключением руководителя)</returns>
        private int FindEmployeesSalary(int DepartmentId)
        {
            int WorkersSalarySum = 0; //переменная в которую будем суммировать зп сотрудников
            for (int i = 0; i < Workers.Count; i++) //в цикле перебираем все сотрудников
            {
                //если Id сотрудника совпадает с Id департамента и сотрудник не является экземпляром класса Manager
                if (Workers[i].DepartmentId == DepartmentId && !(Workers[i] is Manager))
                {
                    WorkersSalarySum += Workers[i].Salary; //то зп данного сотрудника добавляем в сумму
                }
            }
            return WorkersSalarySum; //возвращаем сумму зп сотрудников отдела

        }
        #endregion
    }
}
