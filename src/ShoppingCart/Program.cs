using ShoppingCart;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DI
builder.Services.AddSingleton<IShoppingCartStore, InMemoryShoppingCartStore>();
builder.Services.AddHttpClient<ProductCatalogClient>();
builder.Services.AddSingleton<IEventStore, EventStore>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
