using Base.Contracts.Message;

namespace Base.Message;

/// <summary>
/// Well-known <c>source</c> slugs for the core platform hubs. Products supply their own source string and are
/// not listed here. <see cref="IBaseEventEnvelope{TContent}.Source"/> is a plain string: it is not an enum and
/// the package does not validate it against this list.
/// </summary>
public static class DefaultSources
{
    public const string MessageHub = "message-hub";
    public const string EmailHub = "email-hub";
    public const string IdentityHub = "identity-hub";
    public const string ErrorHub = "error-hub";
    public const string AiHub = "ai-hub";
}
