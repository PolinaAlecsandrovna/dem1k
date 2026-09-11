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

namespace dem1k
{
    
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string pass = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pass))
            {
                lblError.Text = "Введите логин и пароль!";
                return;
            }

            if (DbHelper.AuthUser(login, pass, out string userName, out string role))
            {
                MainWindow mainWindow = new MainWindow(userName, role);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                lblError.Text = "Неверный логин или пароль.";
            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow("Гость", "Гость");
            mainWindow.Show();
            this.Close();
        }
    }
}
