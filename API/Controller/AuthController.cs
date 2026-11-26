using Microsoft.AspNetCore.Mvc;
using API.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using API.Interfaces;

namespace API.Controller
{
    [ApiController]
    [Route("auth")]
    public class AuthController(IAuthService auth) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await auth.RegisterAsync(dto);

            return result.Status switch
            {
                RegisterStatus.Ok => Ok(new
                {
                    ID = result.User!.ID,
                    Name = result.User.Name,
                    Login = result.User.Login
                }),
                RegisterStatus.LoginToken => Conflict("Пользователь с таким логином уже существует"),
                RegisterStatus.InvalidData => BadRequest("Имя, Логин и Пароль обязательны, пароль до 72 символов"),
                _ => Problem("В таблице Role нет такой роли")
            };
        }
    

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var response = await auth.LoginAsync(dto);
            if (response is null)
                return Unauthorized("Неверный логин или пароль");

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me() => Ok(new
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Login = User.FindFirstValue(ClaimTypes.Name),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
