using System.Net.Security;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Base.Message.RabbitMQ;

/// <summary>
/// Manages a single shared RabbitMQ connection and creates channels.
/// RabbitMQ channels are not thread-safe, so each publisher and consumer owns its own channel created here.
/// </summary>
public class RabbitMqConnectionManager : IAsyncDisposable, IDisposable
{
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private readonly RabbitMqOptions _options;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

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
    /// Gets the shared open RabbitMQ connection, creating it when needed.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to an open RabbitMQ connection.</returns>
    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
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

            if (_options.UseTls)
            {
                factory.Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = _options.Host,
                    AcceptablePolicyErrors = _options.AcceptInvalidTlsCertificate
                        ? SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors
                        : SslPolicyErrors.None
                };
            }

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ connection established to '{Host}:{Port}'", _options.Host, _options.Port);

            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <summary>
    /// Creates a new channel on the shared connection. The caller owns the returned channel and is
    /// responsible for disposing it. No topology is declared here: exchanges, queues and bindings exist
    /// before the app starts and are managed outside this package.
    /// </summary>
    /// <param name="options">Optional channel creation options, for example to enable publisher confirmations.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that resolves to a new open RabbitMQ channel.</returns>
    public async Task<IChannel> CreateChannelAsync(
        CreateChannelOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync(options, cancellationToken);
    }

    /// <summary>
    /// Asynchronously releases the shared RabbitMQ connection.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        _connectionLock.Dispose();
    }

    /// <summary>
    /// Releases the shared RabbitMQ connection.
    /// </summary>
    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
        _connectionLock.Dispose();
    }
}