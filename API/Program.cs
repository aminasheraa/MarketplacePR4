using API.Intarfaces;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MarketPlacePR4Context>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler
    = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISavedProductService, SavedProductService>();

var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = jwtKey
    });
builder.Services.AddAuthorization();
using API.Interfaces;
using API.Models;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<MarketPlacePR4Context>();

builder.Services.AddScoped<ICartService,CartService>();

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.MapControllers();


app.UseHttpsRedirection();

app.Run();


