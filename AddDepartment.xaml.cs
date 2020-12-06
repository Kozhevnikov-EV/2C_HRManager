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

namespace Homework_12_ver_1
{
    /// <summary>
    /// Логика взаимодействия для AddDepartment.xaml
    /// </summary>
    public partial class AddDepartment : Window
    {
        #region Свойства класса
        /// <summary>
        /// Текущий экземпляр организации в MainWindow
        /// </summary>
        internal Organization organization = MainWindow.winReference.Organization;

        internal Type Parent = null; //тип вышестоящего (депаратамент или организация), используется при добавлении департамента
        #endregion

        public AddDepartment()
        {
            InitializeComponent();
            DepBox.ItemsSource = organization.Departments;

        }

        /// <summary>
        /// Обработка события выбора Организации в RadioButton
        /// </summary>
        private void RadioButton_Checked_Organization(object sender, RoutedEventArgs e)
        {
            txtParent.Visibility = Visibility.Collapsed;
            DepBox.Visibility = Visibility.Collapsed;
            Add.Visibility = Visibility.Visible;
            Parent = typeof(Organization);
        }

        /// <summary>
        /// Обработка события выбора Департамента в RadioButton
        /// </summary>
        private void RadioButton_Checked_Department(object sender, RoutedEventArgs e)
        {
            txtParent.Visibility = Visibility.Visible;
            DepBox.Visibility = Visibility.Visible;
            Add.Visibility = Visibility.Visible;
            Parent = typeof(Department);
        }

        /// <summary>
        /// Обработка события нажатия кнопки Сохранить
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // В зависимости от условий создаем департамент подчиненный организации или другому департаменту
            if (Parent == typeof(Organization)) { organization.Add_Department(Name.Text, 0); }
            else if (Parent == typeof(Department) && DepBox.SelectedItem != null) 
            { 
                organization.Add_Department(Name.Text, (DepBox.SelectedItem as Department).Id); 
            }
            MainWindow.winReference.CreateTreeView(organization); //обновляем список департаментов в основном окне
            Close(); //закрываем окну
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
