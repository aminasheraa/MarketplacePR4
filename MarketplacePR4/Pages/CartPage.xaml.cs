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
    public partial class CartPage : Page
    {
        private const string ApiUrl = "https://localhost:7151/api/Cart/";

        private static readonly HttpClient Client = new HttpClient();

        private readonly JsonSerializerOptions jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };


        public CartPage()
        {
            InitializeComponent();
        }

        private async void CartPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCart();
        }

        private void SetAuthorization()
        {
            if (string.IsNullOrWhiteSpace(Session.Token))
            {
                throw new Exception(
                    "Токен авторизации не получен.");
            }

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token);
        }

        private async Task LoadCart()
        {

            SetAuthorization();

            HttpResponseMessage response =
                await Client.GetAsync(ApiUrl + "My");

            response.EnsureSuccessStatusCode();

            List<CartItemDTO> cart = await response.Content.ReadFromJsonAsync<List<CartItemDTO>>(jsonOptions);

            ProductsLB.ItemsSource = cart;

            UpdateTotal(cart);
        }


        private void UpdateTotal(List<CartItemDTO> cart)
        {
            if (cart == null)
                cart = new List<CartItemDTO>();

            int totalQuantity = cart.Sum(x => x.Quantity);

            decimal totalPrice = cart.Sum(x => x.Product.Price * x.Quantity);

            TotalQuantityTb.Text = totalQuantity.ToString();
            PriceTb.Text = totalPrice.ToString("N2") + " ₽";

            OrderBtn.IsEnabled = cart.Count > 0;
        }
        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            CartItemDTO item = GetSelectedItem(sender);

            if (item == null)
                return;

            SetAuthorization();

            PostOrderDTO request = new PostOrderDTO
            {
                ProductID = item.ProductID,
                Quantity = 1
            };

            HttpResponseMessage response = await Client.PostAsJsonAsync(ApiUrl, request);

            response.EnsureSuccessStatusCode();

            await LoadCart();

        }

        private async void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            CartItemDTO item = GetSelectedItem(sender);

            if (item == null)
                return;

            SetAuthorization();

            int newQuantity = item.Quantity - 1;

            if (newQuantity <= 0)
            {
                await DeleteCartItem(item.ID);
                return;
            }

            HttpResponseMessage response = await Client.PutAsync(ApiUrl + item.ID + "/" + newQuantity, null);

            response.EnsureSuccessStatusCode();

            await LoadCart();

        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            CartItemDTO item = GetSelectedItem(sender);

            if (item == null)
                return;

            MessageBoxResult result = MessageBox.Show("Удалить товар из корзины?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            await DeleteCartItem(item.ID);
        }

        private async Task DeleteCartItem(int id)
        {
            SetAuthorization();

            HttpResponseMessage response =
                await Client.DeleteAsync(ApiUrl + id);

            response.EnsureSuccessStatusCode();

            await LoadCart();

        }

        private CartItemDTO GetSelectedItem(object sender)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (element == null)
                return null;

            return element.DataContext as CartItemDTO;
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderRegistrationPage());
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();

        }

    }


}
