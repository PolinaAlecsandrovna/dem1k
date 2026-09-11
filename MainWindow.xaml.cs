using dem1k.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dem1k
{
    public partial class MainWindow : Window
    {
        private string _userName;
        private string _userRole;

        public MainWindow(string userName, string role)
        {
            InitializeComponent();
            _userName = userName;
            _userRole = role;

            lblUserInfo.Text = _userName;

            LoadProducts();

            if (_userRole == "Гость" || _userRole == "Авторизованный клиент")
            {
               
            }
        }

        private void LoadProducts()
        {
            List<ProductViewModel> products = DbHelper.GetProducts();
            icProducts.ItemsSource = products;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
