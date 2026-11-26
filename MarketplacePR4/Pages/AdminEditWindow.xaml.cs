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
using System.Net.Http;
using MarketplacePR4.Helpers;
using System.Net.Http.Json;



namespace MarketplacePR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminEditWindow.xaml
    /// </summary>

    public partial class AdminEditWindow : Window
    {
        private bool _isEdit;
        private ProductDTO _product = new ProductDTO();

        private CategoryDTO _category = new CategoryDTO();
        private bool _isCategoryEdit;
        public AdminEditWindow()
        {
            InitializeComponent();
        }

        private async Task LoadCategories()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(
                    "https://localhost:7001/api/Category"
                );

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Не удалось загрузить категории");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();

                List<CategoryDTO> categories =
                    System.Text.Json.JsonSerializer.Deserialize<List<CategoryDTO>>(json);

                _product.Categories = categories ?? new List<CategoryDTO>();
            }


        }


        public void ShowProduct()
        {
            ProductPanel.Visibility = Visibility.Visible;
            CategoryPanel.Visibility = Visibility.Collapsed;
        }


        public async Task ShowCategory()
        {
            ProductPanel.Visibility = Visibility.Collapsed;
            CategoryPanel.Visibility = Visibility.Visible;

            _isCategoryEdit = false;
            _category = new CategoryDTO();

            CategoryNameTextBox.Text = "";

            await LoadCategories();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            AdminEditWindow window = new AdminEditWindow();

            window.ShowProduct();
            window.ShowDialog();
        }


        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            AdminEditWindow window = new AdminEditWindow();

            window.ShowCategory();
            window.ShowDialog();
        }

        private async void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_product.Name))
            {
                MessageBox.Show("Введите название товара");
                return;
            }

            ProductDTO productDTO = new ProductDTO
            {
                ID = _product.ID,
                Name = _product.Name,
                CategoryID = _product.CategoryID,
                CategoryName = "",
                Description = _product.Description,
                Price = _product.Price,
                Image = _product.Image,
                Rating = 0,
                PackageCount = _product.PackageCount,
                IsAvailable = _product.IsAvailable,
                StockQuantity = _product.StockQuantity
            };
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response;

                if (_isEdit)
                {
                    response = await client.PutAsJsonAsync(
                        $"https://localhost:7001/api/Product/{_product.ID}",
                        productDTO
                    );
                }
                else
                {
                    response = await client.PostAsJsonAsync(
                        "https://localhost:7001/api/Product",
                        productDTO
                    );
                }

                if (response.IsSuccessStatusCode)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Ошибка сохранения: {(int)response.StatusCode}");
                }
            }
            
        }
    }


}
