using LobbyAPI.Models;
using LobbyAPI.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSingleton<LobbyHandler>();
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();


//app.UseHttpsRedirection();
app.UseWebSockets();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("/gamehub");

//app.Run();

app.Map("/ws/{lobbyCode}", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        string lobbyCode = context.Request.RouteValues["lobbyCode"]?.ToString() ?? "";
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        await handler.Handle(socket, lobbyCode);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
