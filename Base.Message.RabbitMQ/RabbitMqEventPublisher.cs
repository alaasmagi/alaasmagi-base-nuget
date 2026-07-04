using System.Text.Json;
using Base.Contracts.Message;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Base.Message.RabbitMQ;

public class RabbitMqEventPublisher : IBaseEventPublisher
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventPublisher> _logger;

    public RabbitMqEventPublisher(RabbitMqConnectionManager connectionManager, RabbitMqOptions options,
        ILogger<RabbitMqEventPublisher> logger)
    {
        _connectionManager = connectionManager;
        _options = options;
        _logger = logger;
    }

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