using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Логика взаимодействия для EditWorker.xaml
    /// </summary>
    public partial class EditWorker : Window
    {
        #region Свойства класса
        /// <summary>
        /// Текущий экземпляр работника, выбранный в MainWindow
        /// </summary>
        internal Worker Selected_worker = MainWindow.winReference.WorkersList.SelectedItem as Worker;

        /// <summary>
        /// Текущий экземпляр организации в MainWindow
        /// </summary>
        internal Organization organization = MainWindow.winReference.Organization;

        /// <summary>
        /// Словарь типов экземпляров классов-наследников Worker из MainWindow
        /// </summary>
        Dictionary<string, string> TypePairs = MainWindow.winReference.TypePairs;

        /// <summary>
        /// Регулярное выражение, описывающее правило ввода только цифр
        /// </summary>
        public Regex inputRegex = new Regex(@"^[0-9]$");
        #endregion

        public EditWorker()
        {
            InitializeComponent();

            ///Заполняем поля TextBox и подключаем ItemSource для Combobox вкладки "Повысить/Понизить"
            txtName.Text = Selected_worker.Name;
            txtSurname.Text = Selected_worker.Surname;
            txtAge.Text = $"Возраст: {Selected_worker.Age}";
            ActualType.Text = $"Текущий тип работника: {TypePairs[Selected_worker.GetType().Name]}";
            Col_Worker_Type.ItemsSource = TypePairs.Values;

            ///Заполняем поля TextBox и подключаем ItemSource для Combobox вкладки "Перевести"
            txtName2.Text = Selected_worker.Name;
            txtSurname2.Text = Selected_worker.Surname;
            txtAge2.Text = $"Возраст: {Selected_worker.Age}";
            ActualDepartment.Text = $"Текущий департамент работника: {Selected_worker.DepartmentName}";
            Department_List.ItemsSource = organization.Departments;

            ///Заполняем поля TextBox вкладки "Редактировать"
            txtName3.Text = Selected_worker.Name;
            txtSurname3.Text = Selected_worker.Surname;
            txtAge3.Text = Convert.ToString(Selected_worker.Age);
            txtPosition3.Text = Selected_worker.Position;
            txtProjects3.Text = Convert.ToString(Selected_worker.Projects);
            ///И устанавливаем правило ввода символов в TextBox-ы (ввод только цифр)
            txtAge3.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput);
            txtProjects3.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput);
        }

        /// <summary>
        /// Обработчик события PreviewTextInput для элемента TextBox (ввод только цифр)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //проверяем или подходит введенный символ нашему правилу
            Match match = inputRegex.Match(e.Text);
            //и проверяем или выполняется условие
            //если количество символов в строке больше или равно одному либо
            //если введенный символ не подходит нашему правилу
            if ((sender as TextBox).Text.Length >= 2 || !match.Success)
            {
                //то обработка события прекращается и ввода неправильного символа не происходит
                e.Handled = true;
            }
        }

        /// <summary>
        /// Обработка нажатия кнопки Сохранить во вкладке Повысить/Понизить
        /// </summary>
        private void Button_Click_Shift(object sender, RoutedEventArgs e)
        {
            string new_Type_value = Col_Worker_Type.SelectedItem.ToString(); //переменная, хранящая новый тип
            string new_Type = TypePairs.Where(x => x.Value == new_Type_value).FirstOrDefault().Key; //переменная, хранящая ключ для словаря типов
            if (Col_Worker_Type.SelectedItem != null && Col_Worker_Type.SelectedItem.ToString() != Selected_worker.GetType().Name)
            {
                 organization.Changing_Type_of_Class(Selected_worker, new_Type); //вызываем метод изменения типа сотрудника
            }
            MainWindow.winReference.RefreshWorkersList(); //обновляем отображаемую коллекцию работников
            Close(); //закрываем окно
        }

        /// <summary>
        /// Обработка нажатия кнопки Отмена для всех вкладок
        /// </summary>
        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Обработка нажатия кнопки Сохранить во вкладке Перевести (перевод сотрудника в другой департамент)
        /// </summary>
        private void Button_Click_Transfer(object sender, RoutedEventArgs e)
        {
            if (Selected_worker.GetType().Name == "TopManager") //если тип выбранного сотрудника TopManager
            {
                MessageBox.Show("Сотрудники типа 'Высший менеджмент' не могут быть отнесены к департаменту, измените тип работника", 
                    "Ошибка типа работника",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                int new_DepartmentId = (Department_List.SelectedItem as Department).Id;
                organization.Transfer_Worker_to_Department(Selected_worker, new_DepartmentId); //вызываем метод перевода сотрудника
            }
            MainWindow.winReference.RefreshWorkersList(); //обновляем отображаемую коллекцию работников
            Close(); //закрываем окно
        }

        /// <summary>
        /// Обработка нажатия кнопки Сохранить во вкладке Редактировать
        /// </summary>
        private void Button_Click_Edit(object sender, RoutedEventArgs e)
        {
            //проверяем введенные поля возраст и проекты на наличие значений для предотвращения ошибки конвертирования в int
            int age = (txtAge3.Text != "") ? (Convert.ToInt32(txtAge3.Text)) : 0;
            int projects = (txtProjects3.Text != "") ? (Convert.ToInt32(txtProjects3.Text)) : 0;
            //вызываем метод редактирования работника
            organization.Edit_Worker(
                Selected_worker.Id, Selected_worker.DepartmentId, txtName3.Text, txtSurname3.Text, age,
                txtPosition3.Text, Convert.ToInt32(projects));
            MainWindow.winReference.RefreshWorkersList(); //обновляем отображаемую коллекцию работников
            Close(); //закрываем окно
        }
    }
}
