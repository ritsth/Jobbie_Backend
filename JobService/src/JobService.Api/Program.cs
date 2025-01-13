using JobService.Domain.Repositories;
using JobService.Domain.Services;
using JobService.Infrastructure.Repositories;
using JobService.Infrastructure.Config;
using JobService.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from appsettings.json or environment variable
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
                          ?? "Server=mysql_db;Database=jobdb;User=root;Password=root;";

// Register repository and domain services
builder.Services.AddSingleton<IJobRepository>(sp => new JobRepository(connectionString));
builder.Services.AddScoped<IJobService, JobService.Api.Services.JobControlService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize the database at startup
using (var scope = app.Services.CreateScope())
{
    var connection = MySqlDapperConfig.CreateConnection(connectionString);
    try
    {
        DbInitializer.Initialize(connection);
        Console.WriteLine("Database initialization successful.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
    }
    finally
    {
        connection.Dispose();
    }
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable routing and map controllers
app.UseRouting();
app.MapControllers();

app.Run();
