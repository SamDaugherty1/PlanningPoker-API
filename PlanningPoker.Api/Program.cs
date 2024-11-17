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
// Learn more about configuring Swagger/OpenApi at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
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

// Always use CORS in all environments
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapHub<PokerHub>("api/connect").AllowAnonymous();

app.MapControllers();

app.Run();