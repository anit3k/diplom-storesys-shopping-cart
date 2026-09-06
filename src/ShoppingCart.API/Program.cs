using ShoppingCart.Application.Ports;
using ShoppingCart.Application.UseCases.AddItemsToCart;
using ShoppingCart.Application.UseCases.GetCart;
using ShoppingCart.Application.UseCases.RemoveItemsFromCart;
using ShoppingCart.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Ports -> Infrastructure implementations
builder.Services.AddSingleton<ICartRepository, InMemoryCartRepository>();
builder.Services.AddSingleton<IProductCatalogClient, FakeProductCatalogClient>();
builder.Services.AddSingleton<IEventPublisher, ConsoleEventPublisher>();

// Use case handlers
builder.Services.AddScoped<AddItemsToCartHandler>();
builder.Services.AddScoped<GetCartHandler>();
builder.Services.AddScoped<RemoveItemsFromCartHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();