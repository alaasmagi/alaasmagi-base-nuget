using Microsoft.Extensions.DependencyInjection;

namespace Base.Message.RabbitMQ;

/// <summary>
/// Provides dependency injection helpers for RabbitMQ message publishing.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers RabbitMQ publishing services for the configured options.
    /// </summary>
    /// <param name="services">The service collection to register the publisher with.</param>
    /// <param name="options">The RabbitMQ connection and exchange settings.</param>
    /// <returns>The same service collection so additional calls can be chained.</returns>
    public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, RabbitMqOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<RabbitMqConnectionManager>();
        services.AddSingleton<Base.Contracts.Message.IBaseEventPublisher, RabbitMqEventPublisher>();
        return services;
    }
}
