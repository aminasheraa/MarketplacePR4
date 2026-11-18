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
        HttpClient client = new HttpClient();
        int orderID;

        public OrderPage(int id)
        {
            client.BaseAddress = new Uri("http://localhost:5009");
            orderID = id;

            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            var response = await client.GetAsync($"api/order/{orderID}");
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OrderDTO order = await response.Content.ReadFromJsonAsync<OrderDTO>();
            DataContext = order;
            productsList.ItemsSource = order.Products.ToList();

            var statusResponse = await client.GetAsync("api/order/statuses");
            if (!statusResponse.IsSuccessStatusCode)
            {
                string error = await statusResponse.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

                var response = await client.PutAsJsonAsync($"api/order/{orderID}/status", statusDTO);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Статус заказа успешно изменён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
