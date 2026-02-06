using Common.Kafka;
using Common.Resilience;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using POS.API.Middleware;
using POS.Application;
using POS.Infrastructure;
using POS.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341");
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "POS API",
        Version = "v1",
        Description = "Point of Sale API for POS-WMS Integration System",
        Contact = new OpenApiContact
        {
            Name = "POS-WMS Team",
            Email = "team@example.com"
        }
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("PosDatabase")!,
        name: "postgres",
        tags: new[] { "db", "ready" });

// Add application layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Kafka
builder.Services.AddKafkaProducer(builder.Configuration);

// Add Resilience
builder.Services.AddResiliencePolicies();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "POS API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCorrelationId();
app.UseExceptionHandling();
app.UseRequestLogging();

app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PosDbContext>();
    await context.Database.MigrateAsync();
}

// Log startup
Log.Information("Starting POS API...");
Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
