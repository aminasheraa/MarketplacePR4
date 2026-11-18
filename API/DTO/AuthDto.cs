namespace API.DTO
{
    public record RegisterDto(string Name, string Login, string Password);
    public record LoginDto(string Login,string Password);
    public record LoginResponse(string Token, string Name, string Login, string Role);
}
