namespace Base.Message;

/// <summary>
/// Provides default event type names used by base message integrations.
/// </summary>
public static class DefaultMessageActions
{
	public const string UserCreated = "user.created";
	public const string UserDeleted = "user.deleted";
	public const string UserUpdated = "user.updated";
	public const string UserEnabled = "user.enabled";
	public const string UserDisabled = "user.disabled";
	public const string UserVerify = "user.verify";
	public const string User2FaOtp = "user.2fa.otp";
	public const string UserPasswordReset = "user.password.reset";
}
