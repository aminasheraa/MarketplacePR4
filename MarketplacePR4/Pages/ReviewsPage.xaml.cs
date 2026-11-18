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
    /// Логика взаимодействия для ReviewsPage.xaml
    /// </summary>
    public partial class ReviewsPage : Page
    {
        HttpClient client = new HttpClient();
        List<ReviewDTO> reviews;

        public ReviewsPage()
        {
            client.BaseAddress = new Uri("http://localhost:5009");

            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            var response = await client.GetAsync("api/review/List");
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            reviews = await response.Content.ReadFromJsonAsync<List<ReviewDTO>>();
            if (reviews == null || reviews.Count == 0)
            {
                noReviewsTBl.Visibility = Visibility.Visible;
                reviewsList.Visibility = Visibility.Collapsed;
            }
            else
            {
                noReviewsTBl.Visibility = Visibility.Collapsed;

                reviewsList.Visibility = Visibility.Visible;
                reviewsList.ItemsSource = reviews;
            }
        }

        private void ApplyFilters()
        {
            if (reviews != null)
            {
                List<ReviewDTO> sortedReviews = reviews;

                if (!string.IsNullOrWhiteSpace(searchTB.Text))
                {
                    string search = searchTB.Text.Trim().ToLower();
                    sortedReviews = sortedReviews.Where(sr => sr.Description.Contains(search)).ToList();
                }

                if (ratingCB.SelectedIndex == 1)
                    sortedReviews = sortedReviews.OrderBy(sr => sr.Rating).ToList();

                if (ratingCB.SelectedIndex == 2)
                    sortedReviews = sortedReviews.OrderByDescending(sr => sr.Rating).ToList();

                reviewsList.ItemsSource = sortedReviews;
            }
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ratingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private async void delReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            ReviewDTO review = btn.DataContext as ReviewDTO;
            if (review == null) return;

            var response = await client.GetAsync($"api/review/{review.ID}");
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ReviewDTO realReview = await response.Content.ReadFromJsonAsync<ReviewDTO>();
            if (realReview != null)
            {
                MessageBoxResult res = MessageBox.Show($"Вы действительно хотите удалить отзыв №{realReview.ID}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    var responseDel = await client.DeleteAsync($"api/reviews/del/{realReview.ID}");
                    if (!responseDel.IsSuccessStatusCode)
                    {
                        string error = await responseDel.Content.ReadAsStringAsync();
                        MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Отзыв успешно удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                }
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
