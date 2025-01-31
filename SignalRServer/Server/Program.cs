using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddSignalR(options =>
{
	options.MaximumReceiveMessageSize = 128;
	options.StreamBufferCapacity = 13;
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<BroadcastHub>("/broadcast");

app.Run();