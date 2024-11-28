using PlanningPoker.Api.Hubs;
using PlanningPoker.Api.Services;
using PlanningPoker.Api.Repositories;
using PlanningPoker.Auth.Services;
using PlanningPoker.Api.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenApi at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    }
    else
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
                .WithOrigins(
                    "https://thankful-pebble-01b375c10.5.azurestaticapps.net",
                    "https://planningpoker.agiledevelopers.org"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    }
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

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Apply CORS before routing endpoints
app.UseCors("CorsPolicy");

app.MapHub<PokerHub>("api/connect").AllowAnonymous();

app.MapControllers();

app.Run();