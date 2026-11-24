using MarketplacePR4.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace MarketplacePR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для SavedProductPage.xaml
    /// </summary>
    public partial class SavedProductPage : Page
    {
        private const string CartAddUrl = "cart/";
        public SavedProductPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Избранное доступно только после входа в аккаунт");
                NavigationService?.Navigate(new Pages.AuthPage());
                return;
            }
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            InfoText.Text = "Загрузка";
            SavedProductList.ItemsSource = null;

            var result = await Api.SendAsync<List<SavedProductItemDto>>(HttpMethod.Get, "savedproduct");

            if (result.Status == HttpStatusCode.Unauthorized)
            {
                Session.End();
                MessageBox.Show("Сессия истекла, Войдите в аккаунт заново");
                NavigationService?.Navigate(new Pages.AuthPage());
                return;
            }

            if (!result.Ok)
            {
                InfoText.Text = result.Error;
                return;
            }

            SavedProductList.ItemsSource = result.Data.Select(i => new SavedProductCard(i)).ToList();
            InfoText.Text = result.Data.Count == 0 ? "В избранном пока ничего нет" : null;
        }

        private static SavedProductCard CardOf(object sender)
        {
            return (SavedProductCard)((Button)sender).DataContext;
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            var card = CardOf(sender);

            var result = await Api.SendAsync<object>(HttpMethod.Delete, "savedproduct/" + card.ProductId);

            if (!result.Ok && result.Status != HttpStatusCode.NotFound)
            {
                MessageBox.Show(result.Error);
                return;
            }

            await LoadAsync();
        }

        // «Перенести в корзину»: добавить в корзину, а затем убрать из избранного
        private async void ToCart_Click(object sender, RoutedEventArgs e)
        {
            var card = CardOf(sender);

            var add = await Api.SendAsync<object>(HttpMethod.Post, CartAddUrl + card.ProductId);
            if (!add.Ok)
            {
                MessageBox.Show(add.Status == HttpStatusCode.NotFound
                    ? "Корзина недоступна"
                    : add.Error);
                return;
            }

            await Api.SendAsync<object>(HttpMethod.Delete, "savedproduct/" + card.ProductId);
            await LoadAsync();
        }
    }
}
