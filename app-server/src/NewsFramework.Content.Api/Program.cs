using NewsFramework.Content.Api.Endpoints;
using NewsFramework.Content.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IContentRepository, InMemoryContentRepository>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/health"));
app.MapGet("/health", () => Results.Ok(new
{
    service = "newsframework-content-api",
    status = "ok",
    time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
}));

app.MapContentEndpoints();

app.Run();
