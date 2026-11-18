using API.Models;
namespace API.Intarfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
