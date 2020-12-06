using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Homework_11
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Organization Organization; //Объявляем экземпляр класса Организации

        ObservableCollection<Worker> ListOfWorkers; //Объявляем коллекцию сотрудников (используется для отображения в DataGrid)

        public MainWindow()
        {
            InitializeComponent();
            ListOfWorkers = new ObservableCollection<Worker>(); //Инициализируем коллекцию сотрудников
            this.Closing += MainWindow_Closed; //Метод, вызываемый при закрытии программы пользователем
        }

        #region Методы создания TreeView и DataGrid и обработчики событий
        /// <summary>
        /// Основной метод создания элементов TreeView и DataGrid
        /// </summary>
        /// <param name="Organization"></param>
        private void CreateTreeView(Organization Organization)
        {
            Tree.Items.Add(CreateFirstTreeItem(Organization)); //добавляем первый (основной) элемент - экземпляр организации
            WorkersList.ItemsSource = ListOfWorkers; //указываем коллекцию источник для DataGrid
        }

        /// <summary>
        /// Создание первого элемента TreeViewItem
        /// </summary>
        /// <param name="ItOrganization">Экземпляр Organization, помещаемый в элемент TreeViewItem</param>
        /// <returns>Элемент TreeViewItem</returns>
        private TreeViewItem CreateFirstTreeItem(Organization ItOrganization)
        {
            TreeViewItem item = new TreeViewItem(); //объявляем и инициализируем экземпляр TreeViewItem
            item.Header = ItOrganization.Name; //Пишем наименование организации в заголовок
            item.Tag = ItOrganization; //Вкладываем экземпляр Organization в TreeViewItem
            item.Items.Add("Loading...");//добавляем в элемент TreeViewItem вложенный элемент типа string. На него будем ориентироваться
            //при событии раскрытия данного элемента TreeViewItem
            return item; //Возвращаем элемент TreeViewItem
        }

        /// <summary>
        /// Создания элемента TreeViewItem
        /// </summary>
        /// <param name="ItDepartment">Экземпляр департамента, помещаемый в элемент TreeViewItem</param>
        /// <returns>Элемент TreeViewItem</returns>
        public TreeViewItem CreateTreeItem(Department ItDepartment)
        {
            TreeViewItem item = new TreeViewItem(); //объявляем и инициализируем экземпляр TreeViewItem
            item.Header = ItDepartment.Name; //Пишем имя департамента в заголовок
            item.Tag = ItDepartment; //Вкладываем экземпляр Department в TreeViewItem
            item.Items.Add("Loading..."); //добавляем в элемент TreeViewItem вложенный элемент типа string. На него будем ориентироваться
            //при событии раскрытия данного элемента TreeViewItem
            return item; //Возвращаем элемент TreeViewItem
        }

        /// <summary>
        /// Обработка события "раскрытие" - добавления подчиненных департаментов в TreeViewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tree_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem; //раскрываемый элемент TreeView
            ///Если в возвращенном событием элементе TreeView лежит объект Department и в него вложена одна текстовая переменная Loading...
            if (item.Tag is Department && item.Tag is Department && item.Items.Count == 1 && item.Items[0] is string)
            {
                item.Items.Clear(); //очищаем вложенный элементы (Loading...)
                for (int i = 0; i < (item.Tag as Department).SlaveDepartmentId.Count; i++) //в цикле перебираем Id подчиненных департаментов
                                                                                           //экземпляра Department
                {
                    for (int j = 0; j < Organization.Departmets.Count; j++) //в цикле перебираем коллекцию экземпляров Department в Organization
                    {
                        //если находим подчиненный департамент
                        if ((item.Tag as Department).SlaveDepartmentId[i] == Organization.Departmets[j].Id)
                        {
                            //то добавляем его к "раскрытому" элементу TreeView
                            item.Items.Add(CreateTreeItem(Organization.Departmets[j]));
                        }
                    }
                }
            }
            ///здесь все аналогично, только "раскрываемый" элемент TreeView - организация. Используется при раскрытии первого элемента TreeView
            if (item.Tag is Organization && item.Items.Count == 1 && item.Items[0] is string)
            {
                item.Items.Clear();
                for (int i = 0; i < (item.Tag as Organization).SlaveDepartmentId.Count; i++)
                {
                    for (int j = 0; j < Organization.Departmets.Count; j++)
                    {
                        if ((item.Tag as Organization).SlaveDepartmentId[i] == Organization.Departmets[j].Id)
                        {
                            item.Items.Add(CreateTreeItem(Organization.Departmets[j]));
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Обработка события "выбор" элемента TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tree_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem; //переданный объект инициализируем и объявляем как экземпляр TreeViewItem
            int Id; //текущий Id сотрудника
            ListOfWorkers.Clear(); //очищаем коллекцию работников, отображаемую в DataGrid
            if (item.Tag is Department) //если вложенный в экземпляр TreeViewItem объект является экземпляром Department
            {
                Department ItDepartment = item.Tag as Department; //вложенный объект инициализируем и объявляем как экземпляр Department
                Id = ItDepartment.ChiefId; //текущему Id сотрудника присваиваем Id начальника департамента
                var Manager = Organization.Workers.Where(x => x.Id == Id); //передаем из коллекции работников экземпляр Руководителя
                ListOfWorkers.Add(Manager.ToList()[0]); //добавляем его в коллекцию, отображаемую в DataGrid
                for (int i = 0; i < ItDepartment.DepartmentEmployeesId.Count; i++) //далее в цикле аналогично добавляем всех сотрудников департамента
                {
                    Id = ItDepartment.DepartmentEmployeesId[i];
                    var Work = Organization.Workers.Where(x => x.Id == Id);
                    ListOfWorkers.Add(Work.ToList()[0]); //костыль немного, но как то так. Зато вручную не перебирать, кинул в Linq и усе...
                }
            }
            ////если вложенный в экземпляр TreeViewItem объект является экземпляром Organization
            if (item.Tag is Organization)
            {
                //тогда в коллекцию сотрудников добавляем директора
                Organization ItOrganization = item.Tag as Organization;
                Id = ItOrganization.DirectorId;
                var Director = Organization.Workers.Where(x => x.Id == Id);
                ListOfWorkers.Add(Director.ToList()[0]);
            }
        }
        #endregion

        #region Методы обработки событий Menu
        /// <summary>
        /// Событие выбора элемента меню "Открыть"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Clear(); //очищаем коллекцию работников, отображаемых в DataGrid
            Tree.Items.Clear(); //очищаем TreeView
            Organization = Organization.LoadFromJson(@"base.json"); //загружаем организацию с json
            CreateTreeView(Organization); //добавляем элементы загруженной организации в TreeView
            MessageBox.Show("Файл base.json загружен успешно!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            //сообщение, что загрузка прошла успешно.
        }

        /// <summary>
        /// Событие выбора элемента меню "Сохранить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            Organization.SaveToJson(Organization, @"base.json"); //сохраняем текущую организацию в json
            MessageBox.Show("Файл base.json сохранен успешно!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Событие выбора элемента меню "Демонстрация"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Random(object sender, RoutedEventArgs e)
        {
            ListOfWorkers.Clear(); //очищаем коллекцию работников, отображаемых в DataGrid
            Tree.Items.Clear(); //очищаем TreeView
            Organization = new Organization("ООО 'Лесной завод совы'"); //инициализируем новый экземпляр организации с помощью конструктора
            CreateTreeView(Organization); //добавляем элементы загруженной организации в TreeView
            MessageBox.Show("Демонстрационная организация создана успешно!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Событие выбора элемента меню "Выход"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
