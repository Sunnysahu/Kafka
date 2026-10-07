using Scalar.AspNetCore;
using TrackingService.Kafka.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHostedService<ShipmentEventConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.DarkMode = true;
        //options.Theme = ScalarTheme.Mars;
        options.WithTheme(ScalarTheme.DeepSpace);
        options.WithTitle("Tracking Service");
        options.DefaultHttpClient = new();
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.WithSearchHotKey("k");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
