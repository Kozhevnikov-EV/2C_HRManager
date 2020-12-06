using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Свойства класса
        internal Organization Organization;  //Объявляем экземпляр класса Организации

        /// <summary>
        /// Статичный экземпляр MainWindow для доступа из других окон
        /// </summary>
        public static MainWindow winReference { get; private set; }

        /// <summary>
        /// Коллекция-словарь с переводом типов классов-наследников
        /// </summary>
        internal Dictionary<string, string> TypePairs = new Dictionary<string, string>
        {
            {"Intern", "Студент" },
            {"Workman", "Штатный сотрудник" },
            {"Manager", "Руководитель департамента"},
            {"TopManager", "Высший менеджмент" }
        };

        List<Worker> ListOfWorkers; //Объявляем коллекцию сотрудников (используется для отображения в DataGrid)
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            winReference = this; //ссылка на MainWindow для доступа из других окон
            ListOfWorkers = new List<Worker>(); //Инициализируем коллекцию сотрудников
            this.Closing += MainWindow_Closed; //Метод, вызываемый при закрытии программы пользователем
        }

        #region Методы и события Xaml создания экземпляров TreeViewItem и ListView
        /// <summary>
        /// Основной метод создания элементов TreeView и DataGrid
        /// </summary>
        /// <param name="Organization"></param>
        internal void CreateTreeView(Organization Organization)
        {
            Tree.Items.Clear();
            Tree.Items.Add(TreeService.CreateTreeItem(Organization));
            WorkersList.ItemsSource = ListOfWorkers; //указываем коллекцию источник для DataGrid
        }

        /// <summary>
        /// Обработка события "раскрытие" - добавления подчиненных департаментов в TreeViewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tree_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem; //раскрываемый элемент TreeView
            TreeService.CreateSubItems(item, Organization); //обращаемся к статичному методу для создания подчиненных элементов TreeViewItem
        }

        /// <summary>
        /// Обработка события "выбор" элемента TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tree_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem; //переданный объект инициализируем и объявляем как экземпляр TreeViewItem
            List<Worker> NewListOfWorkers = TreeService.CreateWorkersCollection(item, Organization); //c помощью статичного метода заменяем текущую коллекцию ListOfWorkers
            ListOfWorkers.Clear();                                                                         //сотрудниками выбранного в TreeView департамента/организации
            ListOfWorkers.AddRange(NewListOfWorkers);
            WorkersList.Items.Refresh(); //обновляем отображение работников в MainWindow

        }
        #endregion

        #region Методы обработки событий Menu
        /// <summary>
        /// Событие выбора элемента меню "Открыть"
        /// </summary>
        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Clear(); //очищаем коллекцию работников, отображаемых в DataGrid
            Tree.Items.Clear(); //очищаем TreeView
            Organization = Organization.LoadFromJson(@"base.json"); //загружаем организацию с json
            CreateTreeView(Organization); //добавляем элементы загруженной организации в TreeView
            WorkersList.Items.Refresh(); //обновляем отображение работников в MainWindow
            MessageBox.Show("Файл base.json загружен успешно!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            //сообщение, что загрузка прошла успешно.
        }

        /// <summary>
        /// Событие выбора элемента меню "Сохранить"
        /// </summary>
        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            Organization.SaveToJson(Organization, @"base.json"); //сохраняем текущую организацию в json
            MessageBox.Show("Файл base.json сохранен успешно!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Событие выбора элемента меню "Демонстрация"
        /// </summary>
        private void MenuItem_Random(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Clear(); //очищаем коллекцию работников, отображаемых в DataGrid
            Tree.Items.Clear(); //очищаем TreeView
            Organization = new Organization("ООО 'Лесной завод совы'"); //инициализируем новый экземпляр организации с помощью конструктора
            CreateTreeView(Organization); //добавляем элементы загруженной организации в TreeView
            WorkersList.Items.Refresh(); //обновляем отображение работников в MainWindow
            MessageBox.Show("Демонстрационная организация создана успешно!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Событие выбора элемента меню "Выход"
        /// </summary>
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Событие выбора элемента меню "Добавить департамент"
        /// </summary>
        private void MenuItem_Click_AddDepartment(object sender, RoutedEventArgs e)
        {
            if (Organization != null) //если экзепляр организации не null
            {
                AddDepartment addDepartment = new AddDepartment(); //создаем экземпляр окна
                addDepartment.Owner = this; //назначаем владельцем нового окна MainWindow
                addDepartment.Show(); //отображаем окно
            }
            else { MessageBox_Message("Сначала загрузите организацию", "Организация не найдена"); }
        }

        /// <summary>
        /// Событие выбора элемента меню "Добавить работника"
        /// </summary>
        private void MenuItem_Click_AddWorker(object sender, RoutedEventArgs e)
        {
            if (Organization != null)
            {
                AddWorker addWorker = new AddWorker(); //создаем экземпляр окна AddWorker
                addWorker.Owner = this; //назначаем владельцем нового окна MainWindow
                addWorker.Show(); //Выводи окно на экран
            }
            else { MessageBox_Message("Сначала загрузите организацию", "Организация не найдена"); }
        }
        #endregion

        #region Методы обработки событий ContextMenu в TreeView и ListView
        /// <summary>
        /// Удаление выбранного работника
        /// </summary>
        private void DeleteWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersList.SelectedItem != null) //проверка, что работник выбран
            {
                Worker worker = WorkersList.SelectedItem as Worker; //приведение к типу
                Organization.DeleteWorker(worker); //удаление
                ListOfWorkers.Remove(worker); //удаление из текущей отображаемой коллекции
                WorkersList.Items.Refresh();
            }
            else { MessageBox_Message("Работник не выбран", "Ошибка!"); }
        }

        /// <summary>
        /// Изменение выбранного работника
        /// </summary>
        private void EditWorkerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WorkersList.SelectedItem != null) //проверка, что работник выбран
            {
                EditWorker editWorker = new EditWorker(); //создаем экземпляр окна редактирования работника
                editWorker.Owner = this;
                editWorker.Show();
            }
            else { MessageBox_Message("Работник не выбран", "Ошибка!"); }
        }

        /// <summary>
        /// Удаление выбранного департамента
        /// </summary>
        private void DeleteDepartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Tree.SelectedItem != null && (Tree.SelectedItem as TreeViewItem).Tag is Department) //проверка, что департамент выбран
            {
                Organization.Delete_Department((Tree.SelectedItem as TreeViewItem).Tag as Department); //приведение к типу и вызов метода удаления
                CreateTreeView(Organization); //заново инициализируем TreeView
            }
            else { MessageBox_Message("Департамент не выбран", "Ошибка!"); }
        }

        /// <summary>
        /// Изменение выбранного департамента
        /// </summary>
        private void EditDepartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Tree.SelectedItem != null) //проверка, что департамент выбран
            {
                EditDepartment editDepartment = new EditDepartment(); //создаем экземпляр окна для редактирования департамента
                editDepartment.Owner = this;
                editDepartment.Show();
            }
            else { MessageBox_Message("Департамент не выбран", "Ошибка!"); }
        }
        #endregion

        #region Методы обработки событий кнопок "Все сотрудники" и "Нераспределенные"
        /// <summary>
        /// Метод обработки события кнопки "Все сотрудники"
        /// </summary>
        private void Button_Click_ShowAll(object sender, RoutedEventArgs e)
        {
            if (Organization != null && Organization.Workers != null && Organization.Workers.Count != 0)
            {
                ListOfWorkers.Clear(); //очищаем текущую отображаемую коллекцию сотрудников
                ListOfWorkers.AddRange(Organization.Workers); //добавляем в нее всех сотрудников организации
                WorkersList.Items.Refresh(); //обновляем эелемент Xaml
            }
        }

        /// <summary>
        /// Метод обработки события "Нераспределенные"
        /// </summary>
        private void Button_Click_ShowHomeless(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Clear(); //очищаем текущую отображаемую коллекцию сотрудников
            if (Organization != null && Organization.UnallocatedWorkersId.Count != 0 && Organization.UnallocatedWorkersId != null)
            {
                ListOfWorkers = TreeService.CreateUnallocatedWorkersCollection(Organization); //создаем коллекцию с помощью сервиса
                WorkersList.ItemsSource = ListOfWorkers;
            }
            WorkersList.Items.Refresh(); //обновляем
        }
        #endregion

        #region Вспомогательные методы
        /// <summary>
        /// Метод обновления коллекции сотрудников для отображения в WorkersList
        /// </summary>
        internal void RefreshWorkersList()
        {
            if (Tree.SelectedItem != null)
            {
                List<Worker> NewListOfWorkers = TreeService.CreateWorkersCollection(Tree.SelectedItem as TreeViewItem, Organization);
                //c помощью статичного метода заменяем текущую коллекцию ListOfWorkers сотрудниками выбранного в TreeView департамента/организации
                ListOfWorkers.Clear();
                ListOfWorkers.AddRange(NewListOfWorkers);
                WorkersList.Items.Refresh(); //обновляем отображение работников в MainWindow
            }
        }

        /// <summary>
        /// Метод вызова MessageBox для отображения информационных сообщений
        /// </summary>
        /// <param name="Text">Текст сообщения</param>
        /// <param name="Caption">Заголовок сообщения</param>
        public void MessageBox_Message(string Text, string Caption)
        {
            MessageBox.Show(Text, Caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Метод, вызываемый при закрытии программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, CancelEventArgs e)
        {
            ///Создаем экземпляр MessageBox
            var UserAnswer = MessageBox.Show("Сохранить изменения?",
                                $"Выход из программы {this.Title}",
                                MessageBoxButton.YesNoCancel,
                                MessageBoxImage.Question);
            if (UserAnswer == MessageBoxResult.Yes) //если пользователь подтвердил сохранение текущей организации
            {
                Organization.SaveToJson(Organization, @"base.json"); //сохраняем
                e.Cancel = false; //не отменяем закрытие программы
            }
            else if (UserAnswer == MessageBoxResult.No) //если пользователь подтвердил закрытие программы без сохранения
            {
                e.Cancel = false; //закрываем программу
            }
            else //если пользователь отменил закрытие программы
            {
                e.Cancel = true; //отменяем закрытие программы
            }
        }
        #endregion

        #region Сортировка списка сотрудников по нажатию на заголовок
        private void GridViewColumnHeader_ClickId(object sender, RoutedEventArgs e)
        {   //сортируем по возрастанию/убыванию при помощи IComparer
            ListOfWorkers.Sort(
                (SorterFlags.IdFlag ? Worker.SortedBy(SortedCriterion.Id) : Worker.SortedBy(SortedCriterion.IdDescending)));
            WorkersList.Items.Refresh();
        }

        private void GridViewColumnHeader_ClickName(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Sort(
                (SorterFlags.NameFlag ? Worker.SortedBy(SortedCriterion.Name) : Worker.SortedBy(SortedCriterion.NameDescending)));
            WorkersList.Items.Refresh();
        }

        private void GridViewColumnHeader_ClickSurname(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Sort(
                (SorterFlags.SurnameFlag ? Worker.SortedBy(SortedCriterion.Surname) : Worker.SortedBy(SortedCriterion.SurnameDescending)));
            WorkersList.Items.Refresh();
        }

        private void GridViewColumnHeader_ClickAge(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Sort(
               (SorterFlags.AgeFlag ? Worker.SortedBy(SortedCriterion.Age) : Worker.SortedBy(SortedCriterion.AgeDescending)));
            WorkersList.Items.Refresh();
        }

        private void GridViewColumnHeader_ClickPosition(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Sort(
               (SorterFlags.PositionFlag ? Worker.SortedBy(SortedCriterion.Position) : Worker.SortedBy(SortedCriterion.PositionDescending)));
            WorkersList.Items.Refresh();
        }

        private void GridViewColumnHeader_ClickSalary(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Sort(
               (SorterFlags.SalaryFlag ? Worker.SortedBy(SortedCriterion.Salary) : Worker.SortedBy(SortedCriterion.SalaryDescending)));
            WorkersList.Items.Refresh();
        }

        private void GridViewColumnHeader_ClickProjects(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Sort(
               (SorterFlags.ProjectsDescending ? Worker.SortedBy(SortedCriterion.Projects) : Worker.SortedBy(SortedCriterion.ProjectsDescending)));
            WorkersList.Items.Refresh();
        }
        #endregion



        

       

        

        



        


    }
}
