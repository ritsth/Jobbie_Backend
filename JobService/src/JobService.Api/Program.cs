using JobService.Domain.Repositories;
using JobService.Domain.Services;
using JobService.Infrastructure.Repositories;
using JobService.Infrastructure.Config;
using JobService.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JobService.Grpc.Protos;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from appsettings.json or environment variable
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection");


// Register repository and domain services
builder.Services.AddSingleton<IJobRepository>(sp => new JobRepository(connectionString));
builder.Services.AddScoped<IJobService, JobService.Api.Services.JobControlService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowReactLocalhost3000",
        policy =>
        {
            // Allow only a specific origin (React dev server)
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()    // GET, POST, PUT, DELETE etc.
                  .AllowAnyHeader();   // e.g. Content-Type, Authorization
            
        });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080); // HTTP
    //serverOptions.ListenAnyIP(443, listenOptions => listenOptions.UseHttps()); // HTTPS
});


builder.Services.AddGrpcClient<JobAdmin.JobAdminClient>(o =>
{
    // Use the container name or DNS name in Docker Compose (e.g., "jobservicegrpc")
    // and the port exposed (e.g., 8080) within the Docker network.
    // If you expose 5001:8080 externally, you can use http://localhost:5001 for local dev.
    o.Address = new Uri("http://admin-grpc:8080");
});

// Add controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


Console.WriteLine($"Connection string: {connectionString}");
app.Logger.LogInformation($"Connection string: {connectionString}");

// Initialize the database at startup
using (var scope = app.Services.CreateScope())
{
    var connection = MySqlDapperConfig.CreateConnection(connectionString);
    try
    {
        DbInitializer.Initialize(connection);
        app.Logger.LogInformation("Database initialization successful.");
        Console.WriteLine("Database initialization successful.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database initialization failed.");
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
app.UseCors("AllowReactLocalhost3000");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.MapControllers();

app.Run();
