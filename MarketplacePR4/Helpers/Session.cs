using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarketplacePR4.Helpers
{
    // Кто сейчас вошёл в приложение. Хранится, пока программа открыта.
    public static class Session
    {
        public static string Token { get; private set; }
        public static string Name { get; private set; }
        public static string Login { get; private set; }
        public static string Role { get; private set; }

        public static bool IsLoggedIn { get { return !string.IsNullOrEmpty(Token); } }
        public static bool IsAdmin { get { return Role == "Администратор"; } }

        // Срабатывает при входе и выходе, чтобы окно обновило шапку
        public static event Action Changed;

        // Вход: отправляет логин и пароль, при успехе запоминает пользователя
        public static async Task<ApiResult<LoginResponseDto>> LoginAsync(string login, string password)
        {
            var result = await Api.SendAsync<LoginResponseDto>(HttpMethod.Post, "auth/login",
                new LoginRequestDto { Login = login, Password = password });

            if (result.Ok)
                Start(result.Data);

            return result;
        }

        private static void Start(LoginResponseDto r)
        {
            Token = r.Token;
            Name = r.Name;
            Login = r.Login;
            Role = r.Role;
            Api.SetToken(r.Token);
            Changed?.Invoke();
        }

        public static void End()
        {
            Token = Name = Login = Role = null;
            Api.SetToken(null);
            Changed?.Invoke();
        }
    }
}
