using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Base.Message.RabbitMQ;

public class RabbitMqConnectionManager : IDisposable
{
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConnectionManager(RabbitMqOptions options, ILogger<RabbitMqConnectionManager> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        _logger.LogInformation("RabbitMQ connection established, exchange '{Exchange}' ready", _options.Exchange);

        return _channel;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}