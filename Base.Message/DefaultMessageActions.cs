namespace Base.Message;

/// <summary>
/// Provides the default identity <c>action</c> names published on the platform. Each action is a single
/// hyphenated segment so every routing key stays two segments (<c>{tenant|source}.{action}</c>).
/// </summary>
public static class DefaultMessageActions
{
    public const string UserCreated = "user-created";
    public const string UserDeleted = "user-deleted";
    public const string UserUpdated = "user-updated";
    public const string UserEnabled = "user-enabled";
    public const string UserDisabled = "user-disabled";
    public const string UserVerify = "user-verify";
    public const string User2FaOtp = "user-2fa-otp";
    public const string UserPasswordReset = "user-password-reset";
}
