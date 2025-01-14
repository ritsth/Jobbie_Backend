using AdminGrpcService.Services;
using Microsoft.AspNetCore.Builder;
using AdminService.Repositories;

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

// Configure the HTTP request pipeline.
app.MapGrpcService<AdminJobService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
