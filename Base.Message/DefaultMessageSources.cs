namespace Base.Message;

/// <summary>
/// Provides well-known message <c>source</c> slugs. The source identifies the publisher of a message and
/// is a single-segment lowercase slug that matches the publisher's broker credentials and topic permissions.
/// </summary>
/// <remarks>
/// There is no <c>type</c> field on the envelope: whether a message is a fact or a command is carried by the
/// exchange it travels on (see <see cref="DefaultExchanges"/>), not duplicated in the payload.
/// </remarks>
public static class DefaultMessageSources
{
    /// <summary>
    /// The identity provider (Keycloak). It is the only publisher whose <c>source</c> differs from the
    /// message's <c>tenant</c>, because it publishes on behalf of a realm it is not.
    /// </summary>
    public const string Identity = "identity";
}
