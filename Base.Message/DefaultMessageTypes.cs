namespace Base.Message;

/// <summary>
/// Provides default event type names used by base message integrations.
/// </summary>
public static class DefaultMessageTypes
{
    /// <summary>
    /// Provides default event type names for identity and account workflows.
    /// </summary>
    public static class Identity
    {
        /// <summary>
        /// Identifies a user-created event.
        /// </summary>
        public const string UserCreated = "user.created";

        /// <summary>
        /// Identifies a user-deleted event.
        /// </summary>
        public const string UserDeleted = "user.deleted";

        /// <summary>
        /// Identifies a user-updated event.
        /// </summary>
        public const string UserUpdated = "user.updated";

        /// <summary>
        /// Identifies a user-enabled event.
        /// </summary>
        public const string UserEnabled = "user.enabled";

        /// <summary>
        /// Identifies a user-disabled event.
        /// </summary>
        public const string UserDisabled = "user.disabled";

        /// <summary>
        /// Identifies a user-role-assigned event.
        /// </summary>
        public const string UserRoleAssigned = "user.role.assigned";

        /// <summary>
        /// Identifies a user-role-removed event.
        /// </summary>
        public const string UserRoleRemoved = "user.role.removed";

        /// <summary>
        /// Identifies a client-deleted event.
        /// </summary>
        public const string ClientDeleted = "client.deleted";
        
        /// <summary>
        /// Identifies an email identity-provider link event.
        /// </summary>
        public const string EmailIdentityProviderLink = "email.identity-provider-link";

        /// <summary>
        /// Identifies a password-reset email event.
        /// </summary>
        public const string PasswordReset = "email.password-reset";

        /// <summary>
        /// Identifies a verify-email event.
        /// </summary>
        public const string VerifyEmail = "email.verify";

        /// <summary>
        /// Identifies a two-factor authentication one-time-password email event.
        /// </summary>
        public const string EmailOtp = "email.2fa-otp";
    }
}
