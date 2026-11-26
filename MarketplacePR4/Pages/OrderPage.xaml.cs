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
using System.Net.Http;
using System.Net.Http.Json;
using MarketplacePR4.Helpers;

namespace MarketplacePR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        int orderID;

        public OrderPage(int id)
        {
            orderID = id;

            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            var response = await Api.Http.GetAsync($"api/order/{orderID}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    MessageBox.Show("Необходимо авторизоваться.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    MessageBox.Show("У вас нет прав для просмотра заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Не удалось загрузить заказ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            OrderDTO order = await response.Content.ReadFromJsonAsync<OrderDTO>();
            DataContext = order;
            productsList.ItemsSource = order.Products.ToList();

            var statusResponse = await Api.Http.GetAsync("api/order/statuses");

            if (!statusResponse.IsSuccessStatusCode)
            {
                if (statusResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    MessageBox.Show("Необходимо авторизоваться.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (statusResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    MessageBox.Show("У вас нет прав для просмотра статусов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Не удалось загрузить статусы заказов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            List<StatusDTO> statuses = await statusResponse.Content.ReadFromJsonAsync<List<StatusDTO>>();
            statusCB.ItemsSource = statuses;

            if (Session.IsAdmin)
            {
                saveBtn.Visibility = Visibility.Visible;
                statusCB.Visibility = Visibility.Visible;
                userTBl.Visibility = Visibility.Visible;

                statusTBl.Visibility = Visibility.Collapsed;

                statusCB.SelectedValue = order.StatusID;
            }
            else
            {
                saveBtn.Visibility = Visibility.Collapsed;
                statusCB.Visibility = Visibility.Collapsed;
                userTBl.Visibility = Visibility.Collapsed;

                statusTBl.Visibility = Visibility.Visible;
                statusTBl.Text = statuses.FirstOrDefault(s => s.ID == order.StatusID)?.Name;
            }
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Session.IsAdmin)
            {
                if (statusCB.SelectedValue == null)
                {
                    MessageBox.Show("Выберите статус заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int newStatusID = Convert.ToInt32(statusCB.SelectedValue);

                UpdateOrderStatusDTO statusDTO = new UpdateOrderStatusDTO();
                statusDTO.StatusID = newStatusID;

                var response = await Api.Http.PutAsJsonAsync($"api/order/{orderID}/status", statusDTO);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Статус заказа успешно изменён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        MessageBox.Show("Необходимо авторизоваться.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        MessageBox.Show("У вас нет прав для изменения статуса заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        MessageBox.Show("Некорректный статус заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Не удалось изменить статус заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
