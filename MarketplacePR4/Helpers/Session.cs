using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
