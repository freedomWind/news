using NewsFramework.Media.Api.Endpoints;
using NewsFramework.Media.Api.Repositories;
using NewsFramework.Media.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMediaRepository, PersistentMediaRepository>();
builder.Services.AddSingleton<IMediaObjectStorage, LocalMediaObjectStorage>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/health"));
app.MapGet("/health", () => Results.Ok(new
{
    service = "newsframework-media-api",
    status = "ok",
    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
}));

app.MapMediaEndpoints();

app.Run();
