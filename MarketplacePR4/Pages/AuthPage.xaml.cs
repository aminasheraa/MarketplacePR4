using MarketplacePR4.Helpers;
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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage() : this(null) { }
        public AuthPage(string login)
        {
            InitializeComponent();
            LoginBox.Text = login;

        }

        private async void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = null;
            var login = LoginBox.Text.Trim();
            var password = PasswordBox.Password;

            if (login == null || password == null)
            {
                ErrorText.Text = "Заполните все поля";
                return;
            }

            LoginBtn.IsEnabled = false;
            var result = await Session.LoginAsync(login, password);
            LoginBtn.IsEnabled = true;

            if (!result.Ok)
            {
                ErrorText.Text = result.Error;
                return;
            }
            NavigationService?.Navigate(new Pages.TestPage());
        }

        private void ToAuth_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Pages.RegisterPage());
        }
    }
}
