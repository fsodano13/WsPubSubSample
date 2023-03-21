using System.Reflection;
using Application.Abstractions;
using Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IPubSubService, PubSubService>();
builder.Services.AddSingleton<IClientsService, ClientsService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
