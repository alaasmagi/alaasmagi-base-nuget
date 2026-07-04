using System.Text;
using System.Text.Json;
using Base.Contracts.Message;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Base.Message.RabbitMQ;

/// <summary>
/// Provides a hosted RabbitMQ listener that consumes typed event envelopes from a queue.
/// </summary>
/// <typeparam name="TEvent">The event envelope type consumed by the listener.</typeparam>
public abstract class RabbitMqListenerBase<TEvent> : BackgroundService
    where TEvent : IBaseEventEnvelope
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly IBaseEventHandler<TEvent> _handler;
    private readonly ILogger _logger;
    private readonly string _queueName;
    private readonly string[] _routingKeyPatterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqListenerBase{TEvent}"/> class.
    /// </summary>
    /// <param name="connectionManager">The RabbitMQ connection manager used to acquire a channel.</param>
    /// <param name="options">The RabbitMQ connection and exchange settings.</param>
    /// <param name="handler">The handler that processes deserialized events.</param>
    /// <param name="logger">The logger used to report message processing failures.</param>
    /// <param name="queueName">The queue name to declare and consume from.</param>
    /// <param name="routingKeyPatterns">The routing key patterns to bind to the configured exchange.</param>
    protected RabbitMqListenerBase(
        RabbitMqConnectionManager connectionManager,
        RabbitMqOptions options,
        IBaseEventHandler<TEvent> handler,
        ILogger logger,
        string queueName,
        params string[] routingKeyPatterns)
    {
        _connectionManager = connectionManager;
        _options = options;
        _handler = handler;
        _logger = logger;
        _queueName = queueName;
        _routingKeyPatterns = routingKeyPatterns;
    }

    /// <summary>
    /// Declares the queue bindings and starts consuming RabbitMQ messages until the service is stopped.
    /// </summary>
    /// <param name="stoppingToken">A token that is triggered when the hosted service is stopping.</param>
    /// <returns>A task that represents the lifetime of the background listener.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await _connectionManager.GetChannelAsync(stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        foreach (var pattern in _routingKeyPatterns)
        {
            await channel.QueueBindAsync(_queueName, _options.Exchange, pattern, cancellationToken: stoppingToken);
        }

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.Span);
                var @event = JsonSerializer.Deserialize<TEvent>(json);

                if (@event != null)
                {
                    await _handler.HandleAsync(@event, stoppingToken);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message from queue '{Queue}'", _queueName);
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(_queueName, autoAck: false, consumer, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
