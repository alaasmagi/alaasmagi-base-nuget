namespace Base.Message.RabbitMQ;

/// <summary>
/// Provides RabbitMQ connection and exchange settings for the base message integration.
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
    /// Gets the topic exchange used to publish and consume events.
    /// </summary>
    public required string Exchange { get; init; }

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
