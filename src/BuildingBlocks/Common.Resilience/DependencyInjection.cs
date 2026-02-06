using Common.Resilience.Policies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Common.Resilience;

/// <summary>
/// Dependency injection extensions for resilience policies
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add resilience policies to the service collection
    /// </summary>
    public static IServiceCollection AddResiliencePolicies(this IServiceCollection services)
    {
        // Register policy factory
        services.AddSingleton<IResiliencePolicyFactory, ResiliencePolicyFactory>();

        return services;
    }
}

/// <summary>
/// Factory for creating resilience policies
/// </summary>
public interface IResiliencePolicyFactory
{
    IAsyncPolicy CreateDatabasePolicy();
    IAsyncPolicy CreateKafkaPolicy();
    IAsyncPolicy CreateHttpPolicy();
    IAsyncPolicy CreateCustomPolicy(int maxRetries, TimeSpan breakDuration, TimeSpan timeout);
}

/// <summary>
/// Implementation of resilience policy factory
/// </summary>
public sealed class ResiliencePolicyFactory : IResiliencePolicyFactory
{
    private readonly ILogger<ResiliencePolicyFactory> _logger;

    public ResiliencePolicyFactory(ILogger<ResiliencePolicyFactory> logger)
    {
        _logger = logger;
    }

    public IAsyncPolicy CreateDatabasePolicy()
    {
        var retry = RetryPolicies.CreateDatabaseRetryPolicy(3, _logger);
        var circuitBreaker = CircuitBreakerPolicies.CreateDatabaseCircuitBreaker(5, TimeSpan.FromSeconds(30), _logger);
        var timeout = TimeoutPolicies.CreateOptimisticTimeout(TimeoutPolicies.StandardTimeouts.DatabaseCommand, _logger);

        return Policy.WrapAsync(timeout, retry, circuitBreaker);
    }

    public IAsyncPolicy CreateKafkaPolicy()
    {
        var retry = RetryPolicies.CreateKafkaRetryPolicy(5, _logger);
        var circuitBreaker = CircuitBreakerPolicies.CreateKafkaCircuitBreaker(10, TimeSpan.FromMinutes(1), _logger);
        var timeout = TimeoutPolicies.CreateOptimisticTimeout(TimeoutPolicies.StandardTimeouts.KafkaProduce, _logger);

        return Policy.WrapAsync(timeout, retry, circuitBreaker);
    }

    public IAsyncPolicy CreateHttpPolicy()
    {
        var retry = RetryPolicies.CreateDatabaseRetryPolicy(3, _logger);
        var circuitBreaker = CircuitBreakerPolicies.CreateAdvancedCircuitBreaker(0.5, 10, logger: _logger);
        var timeout = TimeoutPolicies.CreateOptimisticTimeout(TimeoutPolicies.StandardTimeouts.HttpCall, _logger);

        return Policy.WrapAsync(timeout, retry, circuitBreaker);
    }

    public IAsyncPolicy CreateCustomPolicy(int maxRetries, TimeSpan breakDuration, TimeSpan timeout)
    {
        var retryPolicy = RetryPolicies.CreateDatabaseRetryPolicy(maxRetries, _logger);
        var circuitBreakerPolicy = CircuitBreakerPolicies.CreateAdvancedCircuitBreaker(
            0.5, maxRetries * 2, breakDuration: breakDuration, logger: _logger);
        var timeoutPolicy = TimeoutPolicies.CreateOptimisticTimeout(timeout, _logger);

        return Policy.WrapAsync(timeoutPolicy, retryPolicy, circuitBreakerPolicy);
    }
}
