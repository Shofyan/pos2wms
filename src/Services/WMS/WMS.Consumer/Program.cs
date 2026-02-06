using Common.Kafka;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WMS.Consumer.Handlers;
using WMS.Consumer.Workers;
using WMS.Domain.Services;
using WMS.Infrastructure;
using WMS.Infrastructure.Data;

var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Services.AddSerilog();

// Add infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Add Kafka consumer
builder.Services.AddKafkaConsumer(builder.Configuration);

// Add event handlers
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<SaleCompletedEventHandler>();
builder.Services.AddScoped<SaleCancelledEventHandler>();
builder.Services.AddScoped<ReturnCreatedEventHandler>();

// Configure consumer options
builder.Services.Configure<ConsumerOptions>(builder.Configuration.GetSection("Consumer"));

// Add background worker
builder.Services.AddHostedService<PosEventsConsumerWorker>();

var host = builder.Build();

// Apply migrations
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WMS.Infrastructure.Data.WmsDbContext>();
    await context.Database.MigrateAsync();
    
    // Seed data
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<WmsDbContext>>();
    await WmsDbInitializer.SeedAsync(context, logger);
}

Log.Information("Starting WMS Consumer Worker...");

await host.RunAsync();
