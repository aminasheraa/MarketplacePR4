using API.Interfaces;
using API.Models;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<MarketPlacePR4Context>();

builder.Services.AddScoped<ICartService,CartService>();

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

app.MapControllers();


app.UseHttpsRedirection();

app.Run();


