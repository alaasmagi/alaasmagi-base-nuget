using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Base.Message.RabbitMQ;

/// <summary>
/// Manages the shared RabbitMQ connection, channel, and topic exchange declaration.
/// </summary>
public class RabbitMqConnectionManager : IDisposable
{
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConnectionManager"/> class.
    /// </summary>
    /// <param name="options">The RabbitMQ connection and exchange settings.</param>
    /// <param name="logger">The logger used to report connection lifecycle events.</param>
    public RabbitMqConnectionManager(RabbitMqOptions options, ILogger<RabbitMqConnectionManager> logger)
    {
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Gets an open RabbitMQ channel, creating the connection and declaring the configured exchange when needed.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to an open RabbitMQ channel.</returns>
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

    /// <summary>
    /// Releases the RabbitMQ channel and connection.
    /// </summary>
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
