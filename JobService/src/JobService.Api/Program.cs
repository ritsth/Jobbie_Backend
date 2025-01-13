using JobService.Domain.Repositories;
using JobService.Domain.Services;
using JobService.Infrastructure.Repositories;
using JobService.Infrastructure.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using JobService.Infrastructure.Database;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from appsettings.json or environment variable
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
                          ?? "Server=localhost;Database=jobdb;User=root;Password=root;";

// Register repository and domain services
builder.Services.AddSingleton<IJobRepository>(sp => new JobRepository(connectionString));
builder.Services.AddScoped<IJobService, JobService.Api.Services.JobService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger if needed
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database
using(var scope = app.Services.CreateScope())
{
    var jobRepo = scope.ServiceProvider.GetRequiredService<IJobRepository>();
    using var conn = MySqlDapperConfig.CreateConnection(connectionString);
    DbInitializer.Initialize(conn);
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
