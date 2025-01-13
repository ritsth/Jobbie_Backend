using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JobService.Domain.Repositories;
using JobService.Infrastructure.Repositories;
using JobService.Infrastructure.Config;
using JobService.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Read connection string
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection")
                          ?? "Server=localhost;Database=jobdb;User=root;Password=root;";

// Register dependencies
builder.Services.AddSingleton<IJobRepository>(sp => new JobRepository(connectionString));

builder.Services.AddGrpc();

var app = builder.Build();

// Map the gRPC service
app.MapGrpcService<JobGrpcService>();

app.Run();
