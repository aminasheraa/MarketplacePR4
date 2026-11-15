using API.Models;
using API.DTO;
using Microsoft.EntityFrameworkCore;
using API.Intarfaces;


namespace API.Services
{
    public class AuthService(MarketPlacePR4Context db, ITokenService tokens) : IAuthService
    {
        private const string UserRoleName = "Пользователь";

        public async Task<RegisterResult> RegisterAsync(RegisterDto dto)
        {
            var name = dto.Name?.Trim();
            var login = dto.Login?.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(dto.Password)
                || name.Length > 50 || login.Length > 50 || dto.Password.Length > 72)
                return new RegisterResult(RegisterStatus.InvalidData);

            if (await db.Users.AnyAsync(u => u.Login == login))
                return new RegisterResult(RegisterStatus.LoginToken);

            var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == UserRoleName);
            if (role is null) return new RegisterResult(RegisterStatus.RoleNotFound);

            var user = new User
            {
                Name = name,
                Login = login,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleID = role.ID
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return new RegisterResult(RegisterStatus.Ok, user);
        }

        public async Task<LoginResponse?> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrEmpty(dto.Password)) return null;

            var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == dto.Login);

            if (user is null) return null;

            if (!await CheckPasswordAsync(user, dto.Password)) return null;

            return new LoginResponse(tokens.CreateToken(user), user.Name, user.Login, user.Role.Name);
        }

        private async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user.Password.StartsWith("$2"))
                return BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (user.Password != password)
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
