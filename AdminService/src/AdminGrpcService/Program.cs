using AdminGrpcService.Services;
using Microsoft.AspNetCore.Builder;
using AdminService.Infra.Config;
using AdminService.Infra.Database;
using AdminService.Infra.Repositories;
using JobService.Grpc.Protos;
using Grpc.Net.Client;

var builder = WebApplication.CreateBuilder(args);

// Add custom configuration files since the changed the default appsettings.json to appsettings.AdminGrpcService.json
builder.Configuration
    .AddJsonFile("appsettings.AdminGrpcService.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.AdminGrpcService.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


//Database connection string mySQL
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
                          ?? "Server=admin_mysql_db;Database=AdminJobDB;User=Admin;Password=Admin;";

// Dependency injection: Register AdminJobRepository with the connection string
builder.Services.AddScoped<IAdminJobRepository>(serviceProvider => 
    new AdminJobRepository(connectionString));;

// Add services to the container.
builder.Services.AddGrpc();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//Single dependency injection for the JobAdminClient
builder.Services.AddSingleton<JobAdmin.JobAdminClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://localhost:5001"); 
    return new JobAdmin.JobAdminClient(channel);
});


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
