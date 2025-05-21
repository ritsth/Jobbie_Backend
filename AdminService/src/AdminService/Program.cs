using AdminService.Infra.Config;
using AdminService.Infra.Database;
using AdminService.Infra.Repositories;
using JobService.Grpc.Protos;
using AdminService.Clients;
using Microsoft.AspNetCore.Builder;
using Grpc.Net.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Read connection string from appsettings.json or environment variable
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
                          ?? "Server=admin_mysql_db;Database=AdminJobDB;User=Admin;Password=Admin;";

// Dependency injection: Register AdminJobRepository with the connection string
builder.Services.AddScoped<IAdminJobRepository>(serviceProvider => 
    new AdminJobRepository(connectionString));




//Single dependency injection for the JobAdminClient
builder.Services.AddSingleton<JobAdmin.JobAdminClient>(sp =>
{
    var channel = GrpcChannel.ForAddress("http://jobservicegrpc:8080"); 
    return new JobAdmin.JobAdminClient(channel);
});

// Register the AdminJobClient (wrapper for the gRPC client)
builder.Services.AddTransient<AdminJobClient>(sp =>
{
    var jobAdminClient = sp.GetRequiredService<JobAdmin.JobAdminClient>();
    return new AdminJobClient(jobAdminClient);
});



//Frontend cors policy config
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowReactLocalhost3000",
        policy =>
        {
            // Allow only a specific origin (React dev server)
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()    // GET, POST, PUT, DELETE etc.
                  .AllowAnyHeader();   // e.g. Content-Type, Authorization
            
            // If you need credentials or cookies:
            // .AllowCredentials();
        });
});


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


// Enable routing and map controllers
app.UseRouting();
app.UseCors("AllowReactLocalhost3000");
app.MapControllers();

app.Run();
