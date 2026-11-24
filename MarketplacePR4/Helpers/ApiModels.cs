using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplacePR4.Helpers
{
    public class RegisterRequestDto
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    // Что приходит от сервера после входа
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
    }

    // Один товар из избранного (как SavedProduct в API)
    public class SavedProductItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public decimal Rating { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
    }
}
