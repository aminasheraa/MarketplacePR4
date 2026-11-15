using API.DTO;
using API.Models;

namespace API.Intarfaces
{
    public enum RegisterStatus { Ok, InvalidData, LoginToken, RoleNotFound}
    public record RegisterResult(RegisterStatus Status, User? User = null);

    public interface IAuthService
    {
        Task<RegisterResult> RegisterAsync(RegisterDto dto);
        Task<LoginResponse?> LoginAsync(LoginDto dto);
    }
}
