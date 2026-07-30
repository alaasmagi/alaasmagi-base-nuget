using Base.Contracts.Message;

namespace Base.Message;

/// <summary>
/// Builds AMQP routing keys from envelope fields. The routing key is an AMQP property composed at publish
/// time, not a field in the JSON body, and it differs per exchange so that each exchange puts first the
/// field it actually discriminates on.
/// </summary>
public static class BaseRoutingKey
{
    /// <summary>
    /// Builds the routing key for the <see cref="DefaultExchanges.IdentityEvents"/> exchange:
    /// <c>{tenant}.{action}</c>. Every app binds its own realm, so <c>tenant</c> comes first.
    /// </summary>
    /// <typeparam name="TContent">The content payload type carried by the envelope.</typeparam>
    /// <param name="envelope">The event envelope to derive the routing key from.</param>
    /// <returns>The routing key for the identity events exchange.</returns>
    public static string ForIdentityEvents<TContent>(IBaseEventEnvelope<TContent> envelope) =>
        $"{envelope.Tenant}.{envelope.Action}";

    /// <summary>
    /// Builds the routing key for the <see cref="DefaultExchanges.EmailCommands"/> exchange:
    /// <c>{source}.{action}</c>. Every app writes here, so <c>source</c> comes first and is the lever that
    /// topic permissions restrict.
    /// </summary>
    /// <typeparam name="TContent">The content payload type carried by the envelope.</typeparam>
    /// <param name="envelope">The event envelope to derive the routing key from.</param>
    /// <returns>The routing key for the email commands exchange.</returns>
    public static string ForEmailCommands<TContent>(IBaseEventEnvelope<TContent> envelope) =>
        $"{envelope.Source}.{envelope.Action}";
}
