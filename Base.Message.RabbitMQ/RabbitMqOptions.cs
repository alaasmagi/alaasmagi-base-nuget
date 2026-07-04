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
}
