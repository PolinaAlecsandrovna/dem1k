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
   
    public partial class ProductEditWindow : Window
    {
        private ProductViewModel _product;

        public ProductEditWindow(ProductViewModel product)
        {
            InitializeComponent();
            _product = product;

            if (_product != null)
            {
                Title = "Редактирование товара";
                txtName.Text = _product.Name;
                txtPrice.Text = _product.Price.ToString();
                txtQuantity.Text = _product.StockQuantity.ToString();
                txtDiscount.Text = _product.DiscountPercent.ToString();
                txtImagePath.Text = _product.ImagePath;
            }
            else
            {
                Title = "Добавление товара";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите наименование товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Цена должна быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Количество на складе не может быть отрицательным!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtDiscount.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть числом от 0 до 100!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isNew = (_product == null);
            ProductViewModel item = _product ?? new ProductViewModel();

            item.Name = txtName.Text.Trim();
            item.Price = price;
            item.StockQuantity = quantity;
            item.DiscountPercent = discount;
            item.ImagePath = txtImagePath.Text.Trim();

            try
            {
                if (isNew)
                {
                    DbHelper.AddProduct(item);
                    MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    DbHelper.UpdateProduct(item);
                    MessageBox.Show("Товар успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true; 
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении в базу данных:\n{ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
