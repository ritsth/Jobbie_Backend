using AdminService.Config;
using AdminService.Database;
using AdminService.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read connection string from appsettings.json or environment variable
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
                          ?? "Server=mysql_db;Database=AdminJobDB;User=Admin;Password=Admin;";

// Dependency injection: Register AdminJobRepository with the connection string
builder.Services.AddScoped<IAdminJobRepository>(serviceProvider => 
    new AdminJobRepository(connectionString));;


var app = builder.Build();


// Initialize the database at startup for MY SQL
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
        Console.WriteLine($"Database initialization failed: {ex}");
    }
    finally
    {
        connection.Dispose();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
