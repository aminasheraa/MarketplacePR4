
using MarketplacePR4.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MarketplacePR4.Pages
{
    public partial class OrderHistoryPage : Page
    {
        private const string ApiUrl = "https://localhost:7151/api/Order/";

        private static readonly HttpClient Client = new HttpClient();

        private readonly JsonSerializerOptions jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        public OrderHistoryPage()
        {
            InitializeComponent();
        }

        private async void OrderHistoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }

        private void SetAuthorization()
        {
            if (string.IsNullOrWhiteSpace(Session.Token))
            {
                throw new Exception("Токен авторизации не получен");
            }

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token);
        }

        private async Task LoadOrders()
        {
            SetAuthorization();

            HttpResponseMessage response = await Client.GetAsync(ApiUrl + "My");

            response.EnsureSuccessStatusCode();

            List<OrderDTO> orders = await response.Content.ReadFromJsonAsync<List<OrderDTO>>(jsonOptions);

            OrdersLB.ItemsSource = orders;

            if (orders == null || orders.Count == 0)
            {
                MessageBox.Show("У вас пока нет оформленных заказов", "История заказов", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
