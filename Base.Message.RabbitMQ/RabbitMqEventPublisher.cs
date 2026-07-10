using System.Text.Json;
using Base.Contracts.Message;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Base.Message.RabbitMQ;

/// <summary>
/// Publishes base event envelopes to a RabbitMQ exchange as persistent messages with publisher confirmations.
/// </summary>
public class RabbitMqEventPublisher : IBaseEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly SemaphoreSlim _publishLock = new(1, 1);
    private IChannel? _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqEventPublisher"/> class.
    /// </summary>
    public RabbitMqEventPublisher(
        RabbitMqConnectionManager connectionManager,
        RabbitMqOptions options,
        ILogger<RabbitMqEventPublisher> logger)
    {
        _connectionManager = connectionManager;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Publishes an event envelope to RabbitMQ using the topic as the routing key.
    /// </summary>
    public async Task PublishAsync<TContent>(
        string topic,
        IBaseEventEnvelope<TContent> message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Exchange))
        {
            throw new InvalidOperationException(
                "RabbitMQ publishing requires RabbitMqOptions.Exchange to be configured.");
        }

        // A RabbitMQ channel is not thread-safe, so publishes are serialized on a single owned channel.
        await _publishLock.WaitAsync(cancellationToken);
        try
        {
            var channel = await GetChannelAsync(cancellationToken);
            var body = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType());

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: topic,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event with topic '{Topic}'", topic);
            throw;
        }
        finally
        {
            _publishLock.Release();
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        _channel = await _connectionManager.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true),
            cancellationToken);

        return _channel;
    }

    /// <summary>
    /// Asynchronously releases the publisher channel.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        _publishLock.Dispose();
    }
}