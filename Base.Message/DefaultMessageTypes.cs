namespace Base.Message;

public static class DefaultMessageTypes
{
    public static class Identity
    {
        public const string UserCreated = "user.created";
        public const string UserDeleted = "user.deleted";
        public const string UserUpdated = "user.updated";
        public const string UserEnabled = "user.enabled";
        public const string UserDisabled = "user.disabled";
        public const string UserRoleAssigned = "user.role.assigned";
        public const string UserRoleRemoved = "user.role.removed";
        public const string ClientDeleted = "client.deleted";
        
        public const string EmailIdentityProviderLink = "email.identity-provider-link";
        public const string PasswordReset = "email.password-reset";
        public const string VerifyEmail = "email.verify";
        public const string EmailOtp = "email.2fa-otp";
    }
}