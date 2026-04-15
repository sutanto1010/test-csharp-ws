using RealtimeDemo.Api.Services;
using RealtimeDemo.Api.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<INoteService, InMemoryNoteService>();
builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddSingleton<IRealtimeNotifier, WebSocketRealtimeNotifier>();
builder.Services.AddSingleton<NotesWebSocketHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "RealtimeDemo API";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RealtimeDemo API v1");
});

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

app.MapControllers();
app.MapHealthChecks("/health");

app.Map("/ws/notes", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new
        {
            Error = "Expected a WebSocket upgrade request."
        });
        return;
    }

    using var socket = await context.WebSockets.AcceptWebSocketAsync();
    var handler = context.RequestServices.GetRequiredService<NotesWebSocketHandler>();
    await handler.HandleAsync(socket, context.RequestAborted);
});

app.Run();

public partial class Program;
