using Microsoft.Extensions.DependencyInjection;

namespace Base.Message.RabbitMQ;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, RabbitMqOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<RabbitMqConnectionManager>();
        services.AddSingleton<Base.Contracts.Message.IBaseEventPublisher, RabbitMqEventPublisher>();
        return services;
    }
}