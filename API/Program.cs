using API.DTO;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.ReferenceHandler 
= ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<MarketPlacePR4Context>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

const string UserRoleName = "Пользователь";

app.MapPost("/auth/register", async (RegisterDto dto, MarketPlacePR4Context db) => 
{
    var name = dto.Name?.Trim();
    var login = dto.Login?.Trim();

    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(dto.Password))
        return Results.BadRequest("Имя, логин и пароль обязательны");

    if (login.Length > 50 || name.Length > 50) 
        return Results.BadRequest("Логин и имя должны быть не больше 50 символов");

    if(dto.Password.Length > 72) 
        return Results.BadRequest("Пароль должен быть не больше 72 символов");

    if (await db.Users.AnyAsync(u => u.Login == login))
        return Results.Conflict("Пользователь с таким логином уже существует");

    var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == UserRoleName);
    if (role is null) return Results.Problem($"В таблице Role нет роли «{UserRoleName}»");

    var user = new User
    {
        Name = login,
        Login = login,
        Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        RoleID = role.ID
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(new { user.ID, user.Login });

});

app.Run();