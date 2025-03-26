using Darts_Score_Management.Data;
using Darts_Score_Management.Interfaces.RepositoryInterfaces;
using Darts_Score_Management.Interfaces.ServiceInterfaces;
using Darts_Score_Management.Middleware;
using Darts_Score_Management.Repositories;
using Darts_Score_Management.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

//Db
builder.Services.AddDbContext<AppDbContext>((options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
   
});

// Register middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// Register repositories
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGamePlayerRepository, GamePlayerRepository>();
builder.Services.AddScoped<ISetRepository, SetRepository>();
builder.Services.AddScoped<ILegRepository, LegRepository>();
builder.Services.AddScoped<ITurnRepository, TurnRepository>();
builder.Services.AddScoped<IThrowRepository, ThrowRepository>();
builder.Services.AddScoped<ILegStatsRepository, LegStatsRepository>();
builder.Services.AddScoped<ISetStatsRepository, SetStatsRepository>();
builder.Services.AddScoped<IGameStatsRepository, GameStatsRepository>();


// Register services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGamePlayerService, GamePlayerService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<ISetService, SetService>();
builder.Services.AddScoped<ILegService, LegService>();
builder.Services.AddScoped<ITurnService, TurnService>();
builder.Services.AddScoped<IGameValidationService, GameValidationService>();
builder.Services.AddScoped<IGameRulesEngine, GameRulesEngine>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IStatisticService, StatisticService>();


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")  
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseExceptionHandlingMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
