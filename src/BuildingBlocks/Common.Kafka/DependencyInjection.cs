using Common.Kafka.Abstractions;
using Common.Kafka.Configuration;
using Common.Kafka.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Common.Kafka;

/// <summary>
/// Extension methods for registering Kafka services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add Kafka producer and consumer services
    /// </summary>
    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(options =>
        {
            configuration.GetSection(KafkaOptions.SectionName).Bind(options);
        });
        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        return services;
    }

    /// <summary>
    /// Add Kafka producer service only
    /// </summary>
    public static IServiceCollection AddKafkaProducer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(options =>
        {
            configuration.GetSection(KafkaOptions.SectionName).Bind(options);
        });
        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        return services;
    }

    /// <summary>
    /// Add Kafka consumer service
    /// </summary>
    public static IServiceCollection AddKafkaConsumer(
        this IServiceCollection services,
        IConfiguration configuration,
        string? groupId = null)
    {
        services.Configure<KafkaOptions>(options =>
        {
            configuration.GetSection(KafkaOptions.SectionName).Bind(options);
            if (!string.IsNullOrEmpty(groupId))
            {
                options.Consumer.GroupId = groupId;
            }
        });

        services.AddScoped<IKafkaConsumer, KafkaConsumer>();

        return services;
    }
}
