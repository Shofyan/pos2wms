using Common.PostgreSQL.Interceptors;
using Common.PostgreSQL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WMS.Domain.Repositories;
using WMS.Infrastructure.Data;
using WMS.Infrastructure.Repositories;

namespace WMS.Infrastructure;

/// <summary>
/// Dependency injection configuration for the WMS Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<WmsDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("WmsDatabase");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations", "wms");
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            });

            options.UseSnakeCaseNamingConvention();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            // Add interceptors
            var timeProvider = sp.GetRequiredService<TimeProvider>();
            var logger = sp.GetRequiredService<ILogger<AuditInterceptor>>();
            options.AddInterceptors(new AuditInterceptor(timeProvider, logger));
        });

        // Add TimeProvider
        services.AddSingleton(TimeProvider.System);

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        // Add Repositories
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        return services;
    }
}
