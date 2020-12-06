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
using System.CodeDom;
using System.Reflection.Emit;

namespace Homework_12_ver_1
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
        /// Id топменеджеров (директоров)
        /// </summary>
        public ObservableCollection<int> TopManagersId { get; set; }

        /// <summary>
        /// Список Id подчиненных департаментов
        /// </summary>
        public ObservableCollection<int> SlaveDepartmentId { get; set; }

        /// <summary>
        /// Список Id нераспределенных по департаментам сотрудников
        /// </summary>
        public ObservableCollection<int> UnallocatedWorkersId { get; set; }

        /// <summary>
        /// Прибыль организации
        /// </summary>
        public int Profit { get; set; }
        
        /// <summary>
        /// Коллекция всех департаментов организации
        /// </summary>
        public ObservableCollection<Department> Departments { get; set; }

        /// <summary>
        /// Коллекция всех работников организации
        /// </summary>
        public ObservableCollection<Worker> Workers { get; set; }
        #endregion

        /// <summary>
        /// Базовый конструктор, используется для загрузки с Json
        /// </summary>
        [JsonConstructor]
        public Organization()
        { 
        }

        /// <summary>
        /// Конструктор создания демонстрационной организации
        /// </summary>
        /// <param name="name">Имя организации</param>
        public Organization (string name)
        {
            RandomService.RandomOrganization(this, name);
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

        #region Индексаторы
        /// <summary>
        /// Возвращает экземпляр Department или null, если такого экзепляра не существует
        /// </summary>
        /// <param name="DepartmentId">Id экземпляра департамента в экземпляре Organization</param>
        /// <returns>Department</returns>
        public Department this[int DepartmentId]
        {
            get
            {
                Department t = null;
                foreach (var e in Departments)
                {
                    if (e.Id == DepartmentId) { t = e; break; }
                }
                return t;
            }
        }

        /// <summary>
        /// Возвращает экземпляр Worker или null, если такого экземпляра не существует
        /// </summary>
        /// <param name="WorkerId"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        public Worker this[int WorkerId, int DepartmentId]
        {
            get
            {
                Worker t = null;
                foreach (var e in Workers)
                {
                    if (e.Id == WorkerId && e.DepartmentId == DepartmentId) { t = e; break; }
                }
                return t;
            }
        }
        #endregion

        #region Добавление сотрудников
        /// <summary>
        /// Добавление нового экземпляра класса-наследника Worker в Organization
        /// </summary>
        /// <param name="DepartmentId">Id департамента</param>
        /// <param name="worker">Экзепляр класса-наследника Worker</param>
        /// <param name="WorkerType">Тип класса наследника</param>
        public void AddWorker(int DepartmentId, Worker worker, Type WorkerType)
        {
            //условие добавления экземпляра Intern и Workman
            if((WorkerType == typeof(Intern) || WorkerType == typeof(Workman)) && DepartmentId != 0)
            {
                Add_Intern_Workman(DepartmentId, worker);
            }
            else if (WorkerType == typeof(Manager) && DepartmentId != 0) //добавление Manager
            {
                AddManager(DepartmentId, worker);
            }
            else if (WorkerType == typeof(TopManager)) //добавление TopManager
            {
                AddTopManager(worker);
            }
            else if (WorkerType != typeof(TopManager) && DepartmentId == 0) //добавление сотрудника в коллекцию нераспределенных
            {
                Add_Unallocated_Worker(worker);
            }
            SalaryService.MainMethodSetSalaryForManagers(this); //перерасчет ЗП
        }

        /// <summary>
        /// Метод добавления экзепляров Intern и Workman в департамент организации
        /// </summary>
        /// <param name="DepartmentId">Id департамента</param>
        /// <param name="worker">Экземпляр, который добавляем</param>
        private void Add_Intern_Workman(int DepartmentId, Worker worker)
        {
            Department temp = this[DepartmentId]; //объявляем департамент temp, который будет ссылаться на департамент с нужным Id
            temp.DepartmentEmployeesId.Add(worker.Id); //добавляем Id экземпляра Worker в список сотрудников
            worker.DepartmentName = temp.Name; //добавляем название департамента в экзепляр Worker
            worker.DepartmentId = DepartmentId; //добавляем Id Департамента в экземпляр Worker
            Workers.Add(worker); //Добавляем экземпляр Worker в коллекцию всех сотрудников организации
        }

        /// <summary>
        /// Метод добавления экземпляра Manager в департамент организации
        /// </summary>
        /// <param name="DepartmentId">Id департамента</param>
        /// <param name="worker">Экземпляр, который добавляем</param>
        private void AddManager(int DepartmentId, Worker worker)
        {
            Department temp = this[DepartmentId]; //объявляем департамент temp, который будет ссылаться на департамент с нужным Id
            if (temp.ChiefId > 0) //если у департамента уже есть начальник (экземпляр Manager)
            {
                Worker OldChief = this[temp.ChiefId, DepartmentId]; //то перемещаем старого руководителя в нераспределенные
                this[DepartmentId].ChiefId = 0;
                OldChief.DepartmentId = 0;
                OldChief.DepartmentName = "";
                UnallocatedWorkersId.Add(OldChief.Id);
                temp.ChiefId = 0;
            }
            worker.DepartmentName = temp.Name; //добавляем название департамента в экзепляр Worker
            worker.DepartmentId = DepartmentId; //добавляем Id Департамента в экземпляр Worker
            temp.ChiefId = worker.Id; //добавляем Id экземпляра Worker в руководителя департамента
            Workers.Add(worker); //добавляем экземпляр Worker в коллекцию всех сотрудников организации
        }

        /// <summary>
        /// Метод добавления экземпляра TopManager в Organization
        /// </summary>
        /// <param name="worker">Экземпляр, который добавляем</param>
        private void AddTopManager(Worker worker)
        {
            this.TopManagersId.Add(worker.Id); //добавляем Id в список топменеджеров организации
            worker.Salary = Profit; //рассчитываем его ЗП
            Workers.Add(worker); //добавляем экземпляр Worker в коллекцию всех сотрудников организации
        }

        /// <summary>
        /// Метод добавления экземпляра класса-наследника Worker в нераспределенные сотрудники
        /// </summary>
        /// <param name="worker">Экземпляр, который добавляем</param>
        private void Add_Unallocated_Worker(Worker worker)
        {
            worker.DepartmentId = 0; //сбрасываем Id
            worker.DepartmentName = ""; //сбрасываем наименование департамента
            UnallocatedWorkersId.Add(worker.Id); //добавляем в коллекцию нераспределенных
            Workers.Add(worker); //добавляем в коллекцию всех сотрудников организации
        }
        #endregion

        #region Удаление сотрудников
        /// <summary>
        /// Метод удаления экземпляра класса-наследника Worker в Organization
        /// </summary>
        /// <param name="worker">Экземпляр, который удаляем</param>
        public void DeleteWorker(Worker worker)
        {
            //В зависимости от условий выбираем соотвествующий метод удаления
            if (worker.DepartmentId == 0 && worker.GetType() != typeof(TopManager)) { Delete_Unallocated_Worker(worker); } //если нераспределенный
            else if (worker.GetType() == typeof(Intern) || worker.GetType() == typeof(Workman)) { Delete_Intern_Workman(worker); }
            else if (worker.GetType() == typeof(Manager)) { Delete_Manager(worker); }
            else if (worker.GetType() == typeof(TopManager)) { Delete_TopManager(worker); }
            SalaryService.MainMethodSetSalaryForManagers(this); //перерасчет ЗП
        }

        /// <summary>
        /// Метод удаления экзепляров Intern и Workman из департамента организации
        /// </summary>
        /// <param name="worker">Экземпляр, который удаляем</param>
        private void Delete_Intern_Workman(Worker worker)
        {
            int DepartmentId = worker.DepartmentId;
            int Id = worker.Id;
            this[DepartmentId].DepartmentEmployeesId.Remove(Id); //удаляем из списка Id сотрудников департамента
            Workers.Remove(worker); //удаляем экземпляр из коллекции
        }

        /// <summary>
        /// Метод удаления экзепляра Manager из департамента организации
        /// </summary>
        /// <param name="worker">Экземпляр, который удаляем</param>
        private void Delete_Manager(Worker worker)
        {
            int DepartmentId = worker.DepartmentId;
            this[DepartmentId].ChiefId = 0;  //удаляем Id из руководителя департамента
            Workers.Remove(worker); //удаляем экземпляр из коллекции
        }

        /// <summary>
        /// Метод удаления экзепляров TopManager из департамента организации
        /// </summary>
        /// <param name="worker">Экземпляр, который удаляем</param>
        private void Delete_TopManager(Worker worker)
        {
            int Id = worker.Id;
            this.TopManagersId.Remove(Id); //удаляем Id из списка топ менеджеров
            Workers.Remove(worker); //удаляем экземпляр из коллекции
        }

        /// <summary>
        /// Метод удаления экземпляра класса-наследника Worker из нераспределенных сотрудников организации
        /// </summary>
        /// <param name="worker">Экземпляр, который удаляем</param>
        private void Delete_Unallocated_Worker(Worker worker)
        {
            UnallocatedWorkersId.Remove(worker.Id);
            Workers.Remove(worker);
        }
        #endregion

        #region Редактирование сотрудников
        /// <summary>
        /// Изменяет тип экземпляра класса-наследника Worker на другой тип класса-наследника Worker
        /// </summary>
        /// <param name="worker">Исходный экземпляр Worker</param>
        /// <param name="new_Type">Новый тип класса-наследника</param>
        public void Changing_Type_of_Class(Worker worker, string new_Type)
        {
            Worker new_worker; //объявляем новый экземпляр
            if (new_Type == "Intern") { new_worker = new Intern(worker); } //если интерн
            else if (new_Type == "Workman") { new_worker = new Workman(worker); } //если сотрудник
            else if (new_Type == "Manager"){ new_worker = new Manager(worker); } //если руководитель
            else { new_worker = new TopManager(worker); } //если TopManager
            DeleteWorker(worker); //Удаляем исходный экземпляр из Organization
            AddWorker(new_worker.DepartmentId, new_worker, new_worker.GetType()); //Добавляем новый экземпляр в Organization
        }

        /// <summary>
        /// Переводит сотрудника из одного департамента в другой (без смены типа)
        /// </summary>
        /// <param name="worker">Исходный экземпляр Worker</param>
        /// <param name="DepartmentId">Id департамента назначения</param>
        public void Transfer_Worker_to_Department(Worker worker, int DepartmentId)
        {
            Worker new_worker; //объявляем новый экземпляр
            string new_Type = worker.GetType().Name; //получаем класс экземпляра worker
            if (new_Type == "Intern") { new_worker = new Intern(worker); } //если интерн
            else if (new_Type == "Workman") { new_worker = new Workman(worker); } //если сотрудник
            else if (new_Type == "Manager") { new_worker = new Manager(worker); } //если руководитель
            else { new_worker = new TopManager(worker); } //если TopManager
            DeleteWorker(worker); //Удаляем исходный экземпляр из Organization
            AddWorker(DepartmentId, new_worker, new_worker.GetType()); //Добавляем новый экземпляр в Organization
        }

        /// <summary>
        /// Изменяет свойства экземпляра Worker (кроме смены департамента и изменения типа класса)
        /// </summary>
        public void Edit_Worker(int WorkerId, int DepartmetnId, string Name, string Surname, int Age, string Position, int Projects)
        {
            Worker worker = this[WorkerId, DepartmetnId];
            worker.Name = Name;
            worker.Surname = Surname;
            worker.Age = Age;
            worker.Position = Position;
            worker.Projects = Projects;
        }

        /// <summary>
        /// Метод, присваивающий работнику имя департамента, к которому он относится
        /// </summary>
        /// <param name="WorkerId">Id работника</param>
        /// <param name="DepartmentId">Id департамента</param>
        private void Edit_Worker(int WorkerId, int DepartmentId)
        {
            if (this[DepartmentId].DepartmentEmployeesId.Count != 0)
            {
                Worker worker = this[WorkerId, DepartmentId];
                worker.DepartmentName = this[DepartmentId].Name;
            }
        }
        #endregion

        #region Методы добавления, удаления и изменения депаратментов
        /// <summary>
        /// Добавление нового департамента
        /// </summary>
        /// <param name="Name">Наименование</param>
        /// <param name="HigherDepartmentId">Id вышестоящего департамента</param>
        public void Add_Department(string Name, int HigherDepartmentId)
        {
            if (Name == null || Name == "") { Name = "Новый департамент"; } //присвоение имени по умолчанию
            Department department = new Department(Name, HigherDepartmentId); //создаем новый экземпляр департамента
            Departments.Add(department); //добавляем экземпляр в коллекцию департаментов организации
            if (HigherDepartmentId == 0) { SlaveDepartmentId.Add(department.Id); } //если Id вышестоящего департамента 0 - подчиняем организации
            else { this[HigherDepartmentId].SlaveDepartmentId.Add(department.Id); } //или подчиняем указанному департаменту
        }

        /// <summary>
        /// Удаление департамента (сотрудники перемещаются в нераспределенные)
        /// </summary>
        /// <param name="department">Удаляемый экземпляр</param>
        public void Delete_Department(Department department)
        {
            if (department.SlaveDepartmentId.Count != 0)  //если у департамента есть подчиненные департаменты
            {
                //все подчиненные департаменты переподчиняем вышестоящему департаменту
                while (department.SlaveDepartmentId.Count !=0)
                {
                    int Id = department.SlaveDepartmentId.Count - 1;
                    Change_Parent_Department(this[department.SlaveDepartmentId[Id]], department.HigherDepartmentId);
                }
            }
            if (department.ChiefId != 0)
            {
                Add_Unallocated_Worker(this[department.ChiefId, department.Id]); //переводим руководителя в нераспределенные
            }
            if (department.DepartmentEmployeesId.Count != 0)
            {
                foreach (var Id in department.DepartmentEmployeesId) //переводим всех сотрудников в нераспределенные
                {
                    Worker worker = this[Id, department.Id];
                    Add_Unallocated_Worker(worker);
                }
            }
            //удаляем Id департамента из вышестояшего департамента/организации
            if (department.HigherDepartmentId != 0) //удаляем из вышестоящего
            {
                this[department.HigherDepartmentId].SlaveDepartmentId.Remove(department.Id);
            }
            else { SlaveDepartmentId.Remove(department.Id); } //или удаляем из подчиненных напрямую организации
            //удаляем сам департамент
            Departments.Remove(department); //удаляем департамент из коллекции, где хранится его экземпляр
            SalaryService.MainMethodSetSalaryForManagers(this); //пересчитываем ЗП
        }

        /// <summary>
        /// Изменение вышестоящего департамента
        /// </summary>
        /// <param name="department">Экземпляр департамента</param>
        /// <param name="NewHigherDepartmentId">Id нового вышестоящего департамента</param>
        private void Change_Parent_Department(Department department, int NewHigherDepartmentId)
        {
            int OldHigherDepartmentId = department.HigherDepartmentId; //Id старого департамента-родителя
            this[OldHigherDepartmentId].SlaveDepartmentId.Remove(department.Id); //удаляем Id этого департамента из списка подчиненных
                                                                                //в старом родителе
            department.HigherDepartmentId = NewHigherDepartmentId; //добавляем Id нового родителя в наш департамент
            if (NewHigherDepartmentId != 0)
            {
                this[NewHigherDepartmentId].SlaveDepartmentId.Add(department.Id); //добавляем Id нашего департамента в список подчиненных родителя
            }
            else
            {
                SlaveDepartmentId.Add(department.Id); //или подчиняем напрямую организации (старший департамент)
            }
        }

        /// <summary>
        /// Изменение департамента (основной метод)
        /// </summary>
        /// <param name="department">Экземпляр департамента</param>
        /// <param name="NewName">Новое наименование департамента</param>
        /// <param name="NewHigherDepartmentId">Новый вышестоящий департамент</param>
        public void Edit_Department(Department department, string NewName, int NewHigherDepartmentId)
        {
            if (department.Name != NewName) //если изменилось название департамента
            {
                if (NewName == "") { NewName = "Новый департамент"; }
                department.Name = NewName; //меняем название департамента
                Edit_Worker(department.ChiefId, department.Id);
                foreach (var workerId in department.DepartmentEmployeesId)
                {
                    Edit_Worker(workerId, department.Id); //меняем название департамента у работников
                }
            }
            if (department.HigherDepartmentId != NewHigherDepartmentId) //если сменился департамент-родитель
            {
                Change_Parent_Department(department, NewHigherDepartmentId); //меняем старший департамент
                SalaryService.MainMethodSetSalaryForManagers(this); //пересчитываем ЗП
            }
        }
        #endregion
    }
}
