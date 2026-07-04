using System.Text;
using System.Text.Json;
using Base.Contracts.Message;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Base.Message.RabbitMQ;

public abstract class RabbitMqListenerBase<TEvent> : BackgroundService
    where TEvent : IBaseEventEnvelope
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly RabbitMqOptions _options;
    private readonly IBaseEventHandler<TEvent> _handler;
    private readonly ILogger _logger;
    private readonly string _queueName;
    private readonly string[] _routingKeyPatterns;

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