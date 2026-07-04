using Base.Contracts.Message;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Message;

/// <summary>
/// Provides dependency injection helpers for base message services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a typed event handler for the specified event envelope type.
    /// </summary>
    /// <param name="services">The service collection to register the handler with.</param>
    /// <typeparam name="TEvent">The event envelope type handled by the implementation.</typeparam>
    /// <typeparam name="THandler">The concrete event handler type to register.</typeparam>
    /// <returns>The same service collection so additional calls can be chained.</returns>
    public static IServiceCollection AddEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : IBaseEventEnvelope
        where THandler : class, IBaseEventHandler<TEvent>
    {
        services.AddScoped<IBaseEventHandler<TEvent>, THandler>();
        return services;
    }
}
