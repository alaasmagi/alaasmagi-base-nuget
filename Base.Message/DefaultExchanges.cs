namespace Base.Message;

/// <summary>
/// Provides the well-known broker exchange names used by the platform. The exchange a message is published
/// to carries whether it is a fact (<see cref="IdentityEvents"/>) or a command (<see cref="EmailCommands"/>);
/// the two are kept separate as a security boundary, not merely for organisation.
/// </summary>
public static class DefaultExchanges
{
    /// <summary>Facts ("this happened"). Published by the identity provider only; many consumers.</summary>
    public const string IdentityEvents = "identity.events";

    /// <summary>Commands ("send this email"). Published by the identity provider and any app; one consumer.</summary>
    public const string EmailCommands = "email.commands";

    /// <summary>Dead letters from <see cref="IdentityEvents"/>.</summary>
    public const string IdentityEventsDeadLetter = "identity.events.dlx";

    /// <summary>Dead letters from <see cref="EmailCommands"/>.</summary>
    public const string EmailCommandsDeadLetter = "email.commands.dlx";

    /// <summary>
    /// Alternate exchange for both main exchanges. A message whose routing key matches no binding lands here
    /// instead of being silently discarded, turning a typo into a visible failure.
    /// </summary>
    public const string Unrouted = "unrouted";
}
