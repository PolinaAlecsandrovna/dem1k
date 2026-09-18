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
        private List<ProductViewModel> _allProducts;

        public MainWindow(string userName, string role)
        {
            InitializeComponent();
            _userName = userName;
            _userRole = role;

            lblUserInfo.Text = _userName;

            LoadProducts();
            SetupUIByRole();
        }

        private void SetupUIByRole()
        {
            if (_userRole == "Гость" || _userRole == "Авторизованный клиент")
            {
                FilterPanel.Visibility = Visibility.Collapsed;
                btnOrders.Visibility = Visibility.Collapsed;
                btnAddProduct.Visibility = Visibility.Collapsed;
            }
            else if (_userRole == "Менеджер")
            {
                FilterPanel.Visibility = Visibility.Visible;
                btnOrders.Visibility = Visibility.Visible;
                btnAddProduct.Visibility = Visibility.Collapsed;
            }
            else if (_userRole == "Администратор")
            {
                FilterPanel.Visibility = Visibility.Visible;
                btnOrders.Visibility = Visibility.Visible;
                btnAddProduct.Visibility = Visibility.Visible;
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
        private void LoadManufacturers()
        {
            var manufacturers = _allProducts.Select(p => p.ManufacturerName).Distinct().ToList();
            manufacturers.Insert(0, "Все производители");
            cmbManufacturer.ItemsSource = manufacturers;
            cmbManufacturer.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filtered = _allProducts.AsEnumerable();

            string searchText = txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(p =>
                    p.Name.ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText) ||
                    p.ManufacturerName.ToLower().Contains(searchText));
            }

            if (cmbManufacturer.SelectedItem != null && cmbManufacturer.SelectedItem.ToString() != "Все производители")
            {
                filtered = filtered.Where(p => p.ManufacturerName == cmbManufacturer.SelectedItem.ToString());
            }

            if (cmbSort.SelectedIndex == 1) filtered = filtered.OrderBy(p => p.Price);
            else if (cmbSort.SelectedIndex == 2) filtered = filtered.OrderByDescending(p => p.Price);
            else if (cmbSort.SelectedIndex == 3) filtered = filtered.OrderBy(p => p.StockQuantity);
            else if (cmbSort.SelectedIndex == 4) filtered = filtered.OrderByDescending(p => p.StockQuantity);

            icProducts.ItemsSource = filtered.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void CmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow orderWindow = new OrderWindow(_userRole);
            orderWindow.ShowDialog();
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductEditWindow editWindow = new ProductEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadProducts(); 
            }
        }
    }

}
