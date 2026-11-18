
using MarketplacePR4.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MarketplacePR4.Pages
{
    public partial class OrderRegistrationPage : Page
    {
        private const string CartApiUrl =
            "https://localhost:7151/api/cart/";

        private const string OrderApiUrl =
            "https://localhost:7151/api/Order/";

        private static readonly HttpClient Client = new HttpClient();

        private readonly JsonSerializerOptions jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        private List<CartItemDTO> cartItems = new List<CartItemDTO>();

        public class ReceivingTypeItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class PickUpPointItem
        {
            public int ID { get; set; }
            public string Address { get; set; }
        }

        public OrderRegistrationPage()
        {
            InitializeComponent();
        }

        private void SetAuthorization()
        {
            if (string.IsNullOrWhiteSpace(Session.Token))
                throw new Exception("Токен авторизации не получен.");

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token);
        }

        private async void OrderRegistrationPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCart();
            await LoadReceivingTypes();
            await LoadPickUpPoints();
        }

        private async Task LoadCart()
        {
            SetAuthorization();

            HttpResponseMessage response =
                await Client.GetAsync(CartApiUrl + "My");

            response.EnsureSuccessStatusCode();

            cartItems =
                await response.Content.ReadFromJsonAsync<List<CartItemDTO>>(
                    jsonOptions);

            if (cartItems == null)
                cartItems = new List<CartItemDTO>();

            OrderProductsLB.ItemsSource = cartItems;

            CalculateTotalPrice();

        }

        private void CalculateTotalPrice()
        {
            decimal total = 0;

            foreach (var item in cartItems)
            {
                total += item.Product.Price * item.Quantity;
            }

            TotalPriceTB.Text =
                "Сумма к оплате: " + total.ToString("0.00") + " руб.";
        }

        private async Task LoadReceivingTypes()
        {
            SetAuthorization();

            HttpResponseMessage response = await Client.GetAsync(OrderApiUrl + "ReceivingTypes");

            response.EnsureSuccessStatusCode();

            var types = await response.Content.ReadFromJsonAsync<List<ReceivingTypeItem>>(jsonOptions);

            TypeOfReceivingCB.ItemsSource = types;

            TypeOfReceivingCB.SelectedIndex = -1;

        }

        private async Task LoadPickUpPoints()
        {
            SetAuthorization();

            HttpResponseMessage response = await Client.GetAsync(OrderApiUrl + "PickUpPoints");

            response.EnsureSuccessStatusCode();

            var points = await response.Content.ReadFromJsonAsync<List<PickUpPointItem>>(jsonOptions);

            PickUpPointCB.ItemsSource = points;

            PickUpPointCB.SelectedIndex = -1;

        }


        private void TypeOfReceivingCB_SelectionChanged( object sender, SelectionChangedEventArgs e)
        {
            if (TypeOfReceivingCB.SelectedItem == null)
            {
                PickUpPointSP.Visibility = Visibility.Collapsed;
                DeliverySP.Visibility = Visibility.Collapsed;
                return;
            }

            var selectedType = TypeOfReceivingCB.SelectedItem as ReceivingTypeItem;

            if (selectedType == null)
                return;

            if (selectedType.ID == 1)
            {
                PickUpPointSP.Visibility = Visibility.Visible;
                DeliverySP.Visibility = Visibility.Collapsed;
            }
            else if (selectedType.ID == 2)
            {
                PickUpPointSP.Visibility = Visibility.Collapsed;
                DeliverySP.Visibility = Visibility.Visible;
            }
        }

        private async void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста.", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (TypeOfReceivingCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите способ получения заказа.");
                return;
            }

            var selectedType = TypeOfReceivingCB.SelectedItem as ReceivingTypeItem;

            int? pickUpPointId = null;
            string address = null;

            if (selectedType.ID == 1)
            {
                if (PickUpPointCB.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пункт самовывоза.");
                    return;
                }

                var selectedPoint =
                    PickUpPointCB.SelectedItem as PickUpPointItem;

                pickUpPointId = selectedPoint.ID;
            }
            else if (selectedType.ID == 2)
            {
                address = AddressTB.Text.Trim();

                if (string.IsNullOrWhiteSpace(address))
                {
                    MessageBox.Show("Введите адрес доставки.");
                    return;
                }
            }

            SetAuthorization();

            var products = cartItems.Select(item => new
            {
                ProductID = item.ProductID,
                Quantity = item.Quantity
            }).ToList();

            string requestUrl = OrderApiUrl + "?typeOfReceivingId=" + selectedType.ID;

            if (pickUpPointId.HasValue)
            {
                requestUrl += "&pickUpPointId=" + pickUpPointId.Value;
            }

            if (address != null)
            {
                requestUrl += "&address=" + Uri.EscapeDataString(address);
            }

            HttpResponseMessage response = await Client.PostAsJsonAsync(requestUrl, products);

            string responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Не удалось оформить заказ.\n" + responseText, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool cartCleared = true;

            foreach (var item in cartItems)
            {
                HttpResponseMessage deleteResponse = await Client.DeleteAsync(CartApiUrl + item.ID);

                if (!deleteResponse.IsSuccessStatusCode)
                    cartCleared = false;
            }

            if (cartCleared)
            {
                MessageBox.Show("Заказ успешно оформлен!", "Оформление заказа", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Заказ оформлен, но не все товары удалось удалить из корзины", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
