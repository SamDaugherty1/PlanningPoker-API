using PlanningPoker.Api.Hubs;
using PlanningPoker.Api.Services;
using PlanningPoker.Api.Repositories;
using PlanningPoker.Auth.Services;
using PlanningPoker.Api.Cache;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Environment.IsDevelopment() 
    ? new[] { "http://localhost:4200", "https://thankful-pebble-01b375c10.5.azurestaticapps.net" }
    : new[] { "https://thankful-pebble-01b375c10.5.azurestaticapps.net" };

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)); // This is needed for SignalR
});

// Register services
builder.Services.AddSingleton<IActiveGames, ActiveGames>();
builder.Services.AddSingleton<IEstimationRepository, EstimationRepository>();
builder.Services.AddSingleton<IEstimationService, EstimationService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Register SignalR hub
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();

// Always use CORS in all environments - must be after UseRouting and before UseEndpoints
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map endpoints - must be after UseCors
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<PokerHub>("api/connect").AllowAnonymous();
    endpoints.MapControllers();
});

app.Run();