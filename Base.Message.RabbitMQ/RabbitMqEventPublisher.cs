using System.Text.Json;
using Base.Contracts.Message;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Base.Message.RabbitMQ;

/// <summary>
/// Publishes base event envelopes to a RabbitMQ topic exchange.
/// </summary>
public class RabbitMqEventPublisher : IBaseEventPublisher
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventPublisher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqEventPublisher"/> class.
    /// </summary>
    /// <param name="connectionManager">The RabbitMQ connection manager used to acquire a channel.</param>
    /// <param name="options">The RabbitMQ connection and exchange settings.</param>
    /// <param name="logger">The logger used to report publish failures.</param>
    public RabbitMqEventPublisher(RabbitMqConnectionManager connectionManager, RabbitMqOptions options,
        ILogger<RabbitMqEventPublisher> logger)
    {
        _connectionManager = connectionManager;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Publishes an event envelope to RabbitMQ using the topic as the routing key.
    /// </summary>
    /// <param name="topic">The RabbitMQ routing key used to publish the event.</param>
    /// <param name="message">The event envelope to publish.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <typeparam name="TEvent">The event envelope type being published.</typeparam>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public async Task PublishAsync<TEvent>(string topic, TEvent message, CancellationToken cancellationToken = default)
        where TEvent : IBaseEventEnvelope
    {
        try
        {
            var channel = await _connectionManager.GetChannelAsync(cancellationToken);
            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            await channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: topic,
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event with topic '{Topic}'", topic);
            throw;
        }
    }
}
