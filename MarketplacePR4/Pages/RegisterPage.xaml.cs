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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = null;
            var name = NameBox.Text.Trim();
            var login = LoginBox.Text.Trim();
            var password = PasswordBox.Password;

            if(name == null || login == null || password == null)
            {
                ErrorText.Text = "Заполните все поля";
                return;
            }
            if(password != ReapeatBox.Password)
            {
                ErrorText.Text = "Пароли не совпадают";
                return;
            }

            RegisterBtn.IsEnabled = false;
            var result = await Api.SendAsync<object>(HttpMethod.Post, "auth/register",
                new RegisterRequestDto { Name = name, Login = login, Password = password });
            RegisterBtn.IsEnabled = true;

            if (result.Status == HttpStatusCode.Conflict)
            {
                MessageBox.Show("Такой логин уже зарегистрирован. Войдите в аккаунт.");
                NavigationService?.Navigate(new Pages.AuthPage(login));
                return;
            }

            if (!result.Ok)
            {
                ErrorText.Text = result.Error;
                return;
            }
            var loginResult = await Session.LoginAsync(login, password);
            if (!loginResult.Ok)
            {
                ErrorText.Text = loginResult.Error;
                return;
            }

            NavigationService?.Navigate(new Pages.HomePage());
        }

        private void ToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Pages.AuthPage());
        }

    }
}
