using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Логика взаимодействия для AddWorker.xaml
    /// </summary>
    public partial class AddWorker : Window
    {
        #region Свойства класса
        /// <summary>
        /// Текущий экземпляр организации в MainWindow
        /// </summary>
        internal Organization organization = MainWindow.winReference.Organization;

        /// <summary>
        /// Статичное свойство, содержащее текущий создаваемый типа работника (Intern, Workman, Manager, TopManager)
        /// </summary>
        private static Type WorkerType { get; set; }

        /// <summary>
        /// Регулярное выражение, поисывающее правило ввода только цифр
        /// </summary>
        private Regex inputRegex = new Regex(@"^[0-9]$");
        #endregion

        public AddWorker()
        {
            InitializeComponent();
            DepBox.ItemsSource = organization.Departments; //подключаем коллекцию департаментов для отображения в ComboBox
            //подписываем textBox-ы на событие PreviewTextInput, с помощью которого обрабатывем вводимый текст
            Age.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput); 
            Projects.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput);
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
        /// Обработка события нажатия кнопки "Сохранить"
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //проверяем часть полей и ComboBox имеют значения, если нет, то присваиваем значения по умолчанию (0)
            int DepartmentId = (DepBox.SelectedItem != null) ? (DepBox.SelectedItem as Department).Id : 0;
            int age = (Age.Text != "") ? (Convert.ToInt32(Age.Text)) : 0;
            int projects = (Projects.Text != "") ? (Convert.ToInt32(Projects.Text)) : 0 ;
            Worker worker; //объявляем экземпляр работника
            if (WorkerType == typeof(Intern)) //далее инициализируем его в записимости от выбранного типа
            {
                worker = new Intern(
                    Name.Text, Surname.Text, age, Position.Text, DepartmentId, projects);
            }
            else if (WorkerType == typeof(Workman))
            {
                worker = new Workman(
                    Name.Text, Surname.Text, age, Position.Text, DepartmentId, projects);
            }
            else if (WorkerType == typeof(Manager))
            {
                worker = new Manager(
                    Name.Text, Surname.Text, age, Position.Text, DepartmentId, projects);
            }
            else
            {
                DepartmentId = 0;
                worker = new TopManager(
                    Name.Text, Surname.Text, age, Position.Text, projects);
            }
            organization.AddWorker(DepartmentId, worker, WorkerType); //добавляем созданный экземпляр работника в организацию
            MainWindow.winReference.RefreshWorkersList(); //обновляем список сотрудников в MainWindow
            this.Close();
        }

        #region Обработка событий RadioButton
        private void RadioButton_Checked_TopManager(object sender, RoutedEventArgs e)
        {
            DepBox.Visibility = Visibility.Collapsed;
            DepLabel.Visibility = Visibility.Collapsed;
            WorkerType = typeof(TopManager);
        }

        private void RadioButton_Checked_Manager(object sender, RoutedEventArgs e)
        {
            DepBox.Visibility = Visibility.Visible;
            DepLabel.Visibility = Visibility.Visible;
            WorkerType = typeof(Manager);
        }

        private void RadioButton_Checked_Workman(object sender, RoutedEventArgs e)
        {
            DepBox.Visibility = Visibility.Visible;
            DepLabel.Visibility = Visibility.Visible;
            WorkerType = typeof(Workman);
        }

        private void RadioButton_Checked_Intern(object sender, RoutedEventArgs e)
        {
            DepBox.Visibility = Visibility.Visible;
            DepLabel.Visibility = Visibility.Visible;
            WorkerType = typeof(Intern);
        }
        #endregion

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
