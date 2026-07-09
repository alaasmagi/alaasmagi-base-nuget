using System.Text;
using System.Text.Json;
using Base.Contracts.Message;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Base.Message.RabbitMQ;

/// <summary>
/// Provides a hosted RabbitMQ listener that consumes event envelopes with a strongly typed content payload from a queue.
/// </summary>
/// <typeparam name="TContent">The content payload type carried by the consumed event envelopes.</typeparam>
public abstract class RabbitMqConsumerBase<TContent> : BackgroundService
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly IBaseEventHandler<TContent> _handler;
    private readonly ILogger _logger;
    private readonly string _queueName;
    private readonly string[] _routingKeyPatterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConsumerBase{TContent}"/> class.
    /// </summary>
    /// <param name="connectionManager">The RabbitMQ connection manager used to acquire a channel.</param>
    /// <param name="options">The RabbitMQ connection and exchange settings.</param>
    /// <param name="handler">The handler that processes deserialized events.</param>
    /// <param name="logger">The logger used to report message processing failures.</param>
    /// <param name="queueName">The queue name to declare and consume from.</param>
    /// <param name="routingKeyPatterns">The routing key patterns to bind to the configured exchange.</param>
    protected RabbitMqConsumerBase(
        RabbitMqConnectionManager connectionManager,
        RabbitMqOptions options,
        IBaseEventHandler<TContent> handler,
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
    /// Gets the maximum number of unacknowledged messages the broker delivers to this consumer at once.
    /// Override to tune throughput versus fairness. Defaults to 10.
    /// </summary>
    protected virtual ushort PrefetchCount => 10;

    /// <summary>
    /// Declares the queue bindings and starts consuming RabbitMQ messages until the service is stopped.
    /// </summary>
    /// <param name="stoppingToken">A token that is triggered when the hosted service is stopping.</param>
    /// <returns>A task that represents the lifetime of the background listener.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await _connectionManager.CreateChannelAsync(cancellationToken: stoppingToken);

        try
        {
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

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: PrefetchCount, global: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.Span);
                    var @event = JsonSerializer.Deserialize<BaseEventEnvelope<TContent>>(json);

                    if (@event != null)
                    {
                        await _handler.HandleAsync(@event, stoppingToken);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message from queue '{Queue}'", _queueName);

                    // Do not requeue: a message that always fails would be redelivered in a tight loop.
                    // Configure a dead-letter exchange on the queue to capture these messages for inspection.
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(_queueName, autoAck: false, consumer, stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when the hosted service is stopping.
        }
        finally
        {
            await channel.DisposeAsync();
        }
    }
}
