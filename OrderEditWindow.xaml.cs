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
    public partial class OrderEditWindow : Window
    {
        private OrderViewModel _order;

        public OrderEditWindow(OrderViewModel order)
        {
            InitializeComponent();
            _order = order;

            cmbStatus.ItemsSource = DbHelper.GetOrderStatuses();

            if (_order != null)
            {
                Title = "Редактирование заказа";
                txtArticle.Text = _order.Article;
                cmbStatus.SelectedItem = _order.StatusName;
                dpOrderDate.SelectedDate = _order.OrderDate;
                dpDeliveryDate.SelectedDate = _order.DeliveryDate;

                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                Title = "Добавление заказа";
                dpOrderDate.SelectedDate = DateTime.Now; 
                cmbStatus.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArticle.Text) || cmbStatus.SelectedItem == null || dpOrderDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните обязательные поля: Артикул, Статус и Дата заказа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (_order == null) 
                {
                    DbHelper.AddOrder(txtArticle.Text, cmbStatus.SelectedItem.ToString(), txtAddress.Text, dpOrderDate.SelectedDate.Value, dpDeliveryDate.SelectedDate);
                    MessageBox.Show("Заказ успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else 
                {
                    DbHelper.UpdateOrder(_order.Id, txtArticle.Text, cmbStatus.SelectedItem.ToString(), dpOrderDate.SelectedDate.Value, dpDeliveryDate.SelectedDate);
                    MessageBox.Show("Заказ успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_order == null) return;

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот заказ?", "Подтверждение удаления",
                                         MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DbHelper.DeleteOrder(_order.Id);
                    MessageBox.Show("Заказ удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении:\n{ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
