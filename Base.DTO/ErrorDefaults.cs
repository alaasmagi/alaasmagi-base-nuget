namespace Base.DTO;

/// <summary>
/// Provides the default error codes and messages used across base packages.
/// </summary>
public static class ErrorDefaults
{
    public static class Codes
    {
        public const string NotFound = "NOT_FOUND";
        public const string UpdateFailed = "UPDATE_FAILED";
        public const string RemoveFailed = "REMOVE_FAILED";
        public const string MapFailed = "MAP_FAILED";
        public const string Forbidden = "FORBIDDEN";
        public const string InvalidPaging = "INVALID_PAGING";
        public const string ConcurrencyConflict = "CONCURRENCY_CONFLICT";
        public const string ConcurrencyTokenRequired = "CONCURRENCY_TOKEN_REQUIRED";
        public const string SoftDeleteFailed = "SOFT_DELETE_FAILED";
        public const string RestoreFailed = "RESTORE_FAILED";
    }

    public static class Messages
    {
        public const string NotFound = "The requested data could not be retrieved.";
        public const string UpdateFailed = "The data could not be updated.";
        public const string RemoveFailed = "The data could not be removed.";
        public const string MapToServiceModelFailed = "The data could not be mapped to the service model.";
        public const string MapToDomainModelFailed = "The data could not be mapped to the domain model.";
        public const string Forbidden = "The requested entity is not accessible for the current actor.";
        public const string InvalidPaging = "The provided paging parameters are invalid.";
        public const string ConcurrencyConflict = "The entity has been modified by another process.";
        public const string ConcurrencyTokenRequired = "A concurrency token is required for this operation.";
        public const string SoftDeleteFailed = "The data could not be soft-deleted.";
        public const string RestoreFailed = "The data could not be restored.";
    }
}
