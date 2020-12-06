using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Статичный класс для создания элементов TreeView
    /// </summary>
    static class TreeService
    {
        /// <summary>
        /// Создание экземпляра TreeViewItem
        /// </summary>
        /// <param name="obj">Экземпляр Organization или Departments</param>
        /// <returns>TreeViewItem</returns>
        public static TreeViewItem CreateTreeItem(object obj)
        {
            TreeViewItem item = null; //объявляем и инициализируем экземпляр TreeViewItem
            if (obj is Organization) //если передан объект - экземпляр Organization
            {
                item = new TreeViewItem();
                item.Header = (obj as Organization).Name; //Пишем имя департамента в заголовок
                item.Tag = (obj as Organization); //Вкладываем экземпляр Department в TreeViewItem
                item.Items.Add("Loading..."); //добавляем в элемент TreeViewItem вложенный элемент типа string. На него будем ориентироваться
                                              //при событии раскрытия данного элемента TreeViewItem
                
            }
            else if (obj is Department) //если передан объект - экземпляр Department
            {
                item = new TreeViewItem();
                item.Header = (obj as Department).Name; //Пишем имя департамента в заголовок
                item.Tag = (obj as Department); //Вкладываем экземпляр Department в TreeViewItem
                item.Items.Add("Loading..."); //добавляем в элемент TreeViewItem вложенный элемент типа string. На него будем ориентироваться
                                              //при событии раскрытия данного элемента TreeViewItem return item; //Возвращаем элемент TreeViewItem
            }
            return item; //возвращаем экземпляр TreeViewItem
        }

        /// <summary>
        /// Добавляет в экземпляр TreeViewItem подчиненные эелементы TreeViewItem (подчиненные элементы содержат экземпляры Department)
        /// </summary>
        /// <param name="item">Экземпляр TreeViewItem</param>
        /// <param name="obj">Экземпляр Organization, из которого будем брать экземпляры Department</param>
        /// <returns>TreeViewItem</returns>
        public static TreeViewItem CreateSubItems(TreeViewItem item, Organization organization)
        {
            ///Если в возвращенном событием элементе TreeView лежит объект Department и в него вложена одна текстовая переменная Loading...
            if (item.Tag is Department && item.Tag is Department && item.Items.Count == 1 && item.Items[0] is string)
            {
                item.Items.Clear(); //очищаем вложенный элементы (Loading...)
                for (int i = 0; i < (item.Tag as Department).SlaveDepartmentId.Count; i++) //в цикле перебираем Id подчиненных департаментов
                                                                                           //экземпляра Department
                {
                    int Id = (item.Tag as Department).SlaveDepartmentId[i];
                    TreeViewItem SubItem = CreateTreeItem(organization[Id]);
                    if (SubItem != null) { item.Items.Add(CreateTreeItem(organization[Id])); }
                }
            }
            ///здесь все аналогично, только "раскрываемый" элемент TreeView - организация. Используется при раскрытии первого элемента TreeView
            if (item.Tag is Organization && item.Items.Count == 1 && item.Items[0] is string)
            {
                item.Items.Clear();
                for (int i = 0; i < (item.Tag as Organization).SlaveDepartmentId.Count; i++)
                {
                    int Id = (item.Tag as Organization).SlaveDepartmentId[i];
                    item.Items.Add(CreateTreeItem(organization[Id]));
                }
            }
            return item;
        }

        /// <summary>
        /// Создает коллекцию работников выбранного департамента/организации
        /// </summary>
        /// <param name="item">Элемен TreeViewItem, содержащий в item.Tag экземпляр Organization или Department</param>
        /// <param name="organization">Экзепляр Organization, содержащая коллекцию работников</param>
        /// <returns>List<Worker></returns>
        public static List<Worker> CreateWorkersCollection(TreeViewItem item, Organization organization )
        {
            List<Worker> ListOfWorkers = new List<Worker>();
            int WorkerId; //текущий Id сотрудника
            if (item.Tag is Department) //если вложенный в экземпляр TreeViewItem объект является экземпляром Department
            {
                Department department = item.Tag as Department; //вложенный объект инициализируем и объявляем как экземпляр Department
                if (department.ChiefId > 0) //если у департамента есть руководитель
                {
                    WorkerId = department.ChiefId; //текущему Id сотрудника присваиваем Id начальника департамента
                    ListOfWorkers.Add(organization[WorkerId, department.Id]); //добавляем его в коллекцию, отображаемую в MainWindow
                }
                for (int i = 0; i < department.DepartmentEmployeesId.Count; i++) //далее в цикле аналогично добавляем всех сотрудников департамента
                {
                    WorkerId = department.DepartmentEmployeesId[i];
                    ListOfWorkers.Add(organization[WorkerId, department.Id]); //добавляем сотрудников в коллекцию ListOfWorkers из Organization c пом. индесатора
                }
            }
            if (item.Tag is Organization) //если вложенный в экземпляр TreeViewItem объект является экземпляром Organization, тогда в коллекцию сотрудников добавляем директора
            {
                foreach (var e in organization.TopManagersId)
                {
                    WorkerId = e;
                    ListOfWorkers.Add(organization[WorkerId, 0]);
                }
                
            }
            return ListOfWorkers;
        }

        /// <summary>
        /// Создает коллекцию нераспределенных работников
        /// </summary>
        /// <param name="organization">Экземпляр организации</param>
        /// <returns>Коллекцию нераспределенных работников List<Worker></returns>
        public static List<Worker> CreateUnallocatedWorkersCollection(Organization organization)
        {
            List<Worker> ListOfWorkers = new List<Worker>();
            if (organization.UnallocatedWorkersId.Count != 0 && organization.UnallocatedWorkersId != null)
            foreach (var Id in organization.UnallocatedWorkersId)
            {
                    ListOfWorkers.Add(organization[Id, 0]);
            }
            return ListOfWorkers;
        }
    }
}
