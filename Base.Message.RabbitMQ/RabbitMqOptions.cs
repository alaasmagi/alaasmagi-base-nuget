namespace Base.Message.RabbitMQ;

/// <summary>
/// Provides RabbitMQ connection and optional exchange settings for the base message integration.
/// </summary>
public class RabbitMqOptions
{
    /// <summary>
    /// Gets the RabbitMQ host name.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Gets the RabbitMQ port.
    /// </summary>
    public int Port { get; init; } = 5672;

    /// <summary>
    /// Gets the RabbitMQ user name.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Gets the RabbitMQ password.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Gets the RabbitMQ virtual host.
    /// </summary>
    public string VirtualHost { get; init; } = "/";

    /// <summary>
    /// Gets the exchange used to publish and consume events.
    /// Leave empty to use queue-only/default-exchange behavior.
    /// </summary>
    public string Exchange { get; init; } = string.Empty;

    /// <summary>
    /// Gets the exchange type to declare when <see cref="Exchange"/> is configured.
    /// </summary>
    public string ExchangeType { get; init; } = RabbitMQ.Client.ExchangeType.Topic;

    /// <summary>
    /// Gets a value indicating whether the configured exchange should be declared by this library.
    /// Set to false when topology is managed externally.
    /// </summary>
    public bool DeclareExchange { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the declared exchange should be durable.
    /// </summary>
    public bool ExchangeDurable { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the declared exchange should auto-delete.
    /// </summary>
    public bool ExchangeAutoDelete { get; init; } = false;

    /// <summary>
    /// Gets a value indicating whether TLS (amqps) should be used for the connection.
    /// </summary>
    public bool UseTls { get; init; } = false;

    /// <summary>
    /// Gets a value indicating whether TLS certificate errors should be accepted.
    /// Set to <c>true</c> when connecting to servers with self-signed certificates.
    /// </summary>
    public bool AcceptInvalidTlsCertificate { get; init; } = false;
}
