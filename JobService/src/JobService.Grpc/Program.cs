using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JobService.Domain.Repositories;
using JobService.Infrastructure.Repositories;
using JobService.Infrastructure.Config;
using JobService.Grpc.Services;
using JobService.Kafka.Services;

var builder = WebApplication.CreateBuilder(args);

// Read connection string
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection")
                          ?? "Server=localhost;Database=jobdb;User=root;Password=root;";

// Register dependencies
builder.Services.AddSingleton<IJobRepository>(sp => new JobRepository(connectionString));

// Register KafkaProducer with necessary configuration
builder.Services.AddSingleton<KafkaProducer>(sp =>
{
    var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
    var topic = builder.Configuration.GetValue<string>("Kafka:Topic");

    return new KafkaProducer(bootstrapServers, topic);
});


builder.Services.AddGrpc();

var app = builder.Build();

// Map the gRPC service
app.MapGrpcService<JobGrpcService>();

app.Run();
