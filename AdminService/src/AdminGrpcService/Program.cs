using AdminGrpcService.Services;
using Microsoft.AspNetCore.Builder;
using AdminService.Repositories;
using AdminService.Config;
using AdminService.Database;

var builder = WebApplication.CreateBuilder(args);

//Database connection string mySQL
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
                          ?? "Server=mysql_db;Database=AdminJobDB;User=Admin;Password=Admin;";

// Dependency injection: Register AdminJobRepository with the connection string
builder.Services.AddScoped<IAdminJobRepository>(serviceProvider => 
    new AdminJobRepository(connectionString));;

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// // Initialize the database at startup for MY SQL
// using (var scope = app.Services.CreateScope())
// {
//     var connection = MySqlDapperConfig.CreateConnection(connectionString);
//     try
//     {
//         DbInitializer.Initialize(connection);
//         Console.WriteLine("Database initialization successful.");
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Database initialization failed: {ex}");
//     }
//     finally
//     {
//         connection.Dispose();
//     }
// }

// Configure the HTTP request pipeline.
app.MapGrpcService<AdminJobService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
