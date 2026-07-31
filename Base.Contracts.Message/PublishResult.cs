namespace Base.Contracts.Message;

/// <summary>
/// Represents the outcome of a publish attempt. A <see cref="IBaseEventPublisher"/> returns this instead of
/// relying on the absence of an exception: a broker call returning without throwing does not mean the broker
/// accepted or routed the message. <see cref="Success"/> is <c>true</c> only once the broker has confirmed
/// the message and it was not returned as unroutable.
/// </summary>
public sealed record PublishResult
{
    /// <summary>
    /// Gets a value indicating whether the broker confirmed the message and did not return it as unroutable.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets a human-readable reason when <see cref="Success"/> is <c>false</c> (for example a confirm
    /// timeout, a broker nack, or an unroutable basic-return); otherwise <c>null</c>.
    /// </summary>
    public string? FailureReason { get; init; }

    /// <summary>Creates a successful result.</summary>
    public static PublishResult Ok() => new() { Success = true };

    /// <summary>Creates a failed result with the given <paramref name="reason"/>.</summary>
    public static PublishResult Failed(string reason) => new() { Success = false, FailureReason = reason };
}
