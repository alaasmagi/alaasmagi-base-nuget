using Base.Contracts.Message;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Message;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : IBaseEventEnvelope
        where THandler : class, IBaseEventHandler<TEvent>
    {
        services.AddScoped<IBaseEventHandler<TEvent>, THandler>();
        return services;
    }
}