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
    /// Логика взаимодействия для EditDepartment.xaml
    /// </summary>
    public partial class EditDepartment : Window
    {
        #region Свойства класса
        /// <summary>
        /// Текущий экземпляр организации в MainWindow
        /// </summary>
        internal Organization organization = MainWindow.winReference.Organization;

        /// <summary>
        /// Текущий экземпляр работника, выбранный в MainWindow
        /// </summary>
        internal Department Selected_department = (MainWindow.winReference.Tree.SelectedItem as TreeViewItem).Tag as Department;

        internal Type Parent = null; //тип вышестоящего (депаратамент или организация), используется при добавлении департамента
        #endregion

        public EditDepartment()
        {
            InitializeComponent();
            DepBox.ItemsSource = organization.Departments;
        }

        /// <summary>
        /// Обработка события RadioButton
        /// </summary>
        private void RadioButton_Checked_Organization(object sender, RoutedEventArgs e)
        {
            txtParent.Visibility = Visibility.Collapsed;
            DepBox.Visibility = Visibility.Collapsed;
            Save.Visibility = Visibility.Visible;
            Parent = typeof(Organization);
        }

        /// <summary>
        /// Обработка события RadioButton
        /// </summary>
        private void RadioButton_Checked_Department(object sender, RoutedEventArgs e)
        {
            txtParent.Visibility = Visibility.Visible;
            DepBox.Visibility = Visibility.Visible;
            Save.Visibility = Visibility.Visible;
            Parent = typeof(Department);
        }

        /// <summary>
        /// Обработка события кнопки сохранить
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Parent == typeof(Organization)) //если выбрано подчинение напрямую организации
            {
                organization.Edit_Department(Selected_department, Name.Text, 0); //вызываем метод редактирования департамента
            }
            else if (Parent == typeof(Department) && DepBox.SelectedItem != null) //если выбрано подчинение департаменту
            {
                //вызываем метод редактирования департамента
                organization.Edit_Department(Selected_department, Name.Text, (DepBox.SelectedItem as Department).Id);
            }
            MainWindow.winReference.CreateTreeView(organization);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
