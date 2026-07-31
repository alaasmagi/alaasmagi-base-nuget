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
    private readonly IBaseEventHandler<TContent> _handler;
    private readonly ILogger _logger;
    private readonly string _queueName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConsumerBase{TContent}"/> class. The queue and its
    /// bindings are part of the externally managed topology and must exist before the consumer starts; this
    /// class only consumes from <paramref name="queueName"/>.
    /// </summary>
    protected RabbitMqConsumerBase(
        RabbitMqConnectionManager connectionManager,
        IBaseEventHandler<TContent> handler,
        ILogger logger,
        string queueName)
    {
        _connectionManager = connectionManager;
        _handler = handler;
        _logger = logger;
        _queueName = queueName;
    }

    /// <summary>
    /// Gets the maximum number of unacknowledged messages the broker delivers to this consumer at once.
    /// Override to tune throughput versus fairness. Defaults to 10.
    /// </summary>
    protected virtual ushort PrefetchCount => 10;

    /// <summary>
    /// Starts consuming RabbitMQ messages from the pre-existing queue until the service is stopped. The queue
    /// and its bindings are not declared here; they are managed outside this package.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await _connectionManager.CreateChannelAsync(cancellationToken: stoppingToken);

        try
        {
            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: PrefetchCount,
                global: false,
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

                    await channel.BasicNackAsync(
                        ea.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        stoppingToken);
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