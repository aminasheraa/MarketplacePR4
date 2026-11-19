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

namespace MarketplacePR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminEditWindow.xaml
    /// </summary>

    public partial class AdminEditWindow : Window
    {
        public AdminEditWindow()
        {
            InitializeComponent();
        }

        public void ShowProduct()
        {
            EditContent.ContentTemplate =
                (DataTemplate)FindResource("ProductTemplate");

            EditContent.Content = new object();
        }

        public void ShowPickup()
        {
            EditContent.ContentTemplate =
                (DataTemplate)FindResource("PickupTemplate");

            EditContent.Content = new object();
        }

        public void ShowCategory()
        {
            EditContent.ContentTemplate =
                (DataTemplate)FindResource("CategoryTemplate");

            EditContent.Content = new object();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            AdminEditWindow window = new AdminEditWindow();

            window.ShowProduct();
            window.ShowDialog();
        }

        private void EditPickup_Click(object sender, RoutedEventArgs e)
        {
            AdminEditWindow window = new AdminEditWindow();

            window.ShowPickup();
            window.ShowDialog();
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            AdminEditWindow window = new AdminEditWindow();

            window.ShowCategory();
            window.ShowDialog();
        }
    }


}
