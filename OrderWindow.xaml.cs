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
using System.Windows.Shapes;

namespace dem1k
{
    public partial class OrderWindow : Window
    {
        private string _roleName;

        public OrderWindow(string roleName)
        {
            InitializeComponent();
            _roleName = roleName;

            if (_roleName == "Администратор")
            {
                btnAddOrder.Visibility = Visibility.Visible;
            }

            LoadOrders();
        }

        private void LoadOrders()
        {
            icOrders.ItemsSource = DbHelper.GetOrders();
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderEditWindow editWindow = new OrderEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadOrders(); 
            }
        }

        private void Order_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_roleName != "Администратор") return;

            var border = sender as Border;
            if (border != null && border.DataContext is OrderViewModel order)
            {
                OrderEditWindow editWindow = new OrderEditWindow(order);
                if (editWindow.ShowDialog() == true)
                {
                    LoadOrders();
                }
            }
        }
    }
}
