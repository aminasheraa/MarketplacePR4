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

namespace MarketplacePR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void savedBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SavedProductPage());
        }

        private void ordersListBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderHistoryPage());
        }

        private void cartBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        private void reviewsBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReviewsPage());
        }
    }
}
