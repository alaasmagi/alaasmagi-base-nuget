namespace Base.Message.RabbitMQ;

/// <summary>
/// Provides RabbitMQ connection settings and the exchange to publish to. Connection settings come from
/// configuration only; no credentials are hard-coded. This package publishes and consumes against topology
/// (exchanges, queues, bindings, permissions) that already exists — it never declares it.
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
    /// Gets the exchange to publish to. Required, with no default: the exchange is part of the externally
    /// managed topology and the publisher will not guess it.
    /// </summary>
    public required string Exchange { get; init; }

    /// <summary>
    /// Gets how long to wait for a broker publisher confirmation before treating a publish as failed.
    /// </summary>
    public TimeSpan PublishConfirmTimeout { get; init; } = TimeSpan.FromSeconds(10);

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
