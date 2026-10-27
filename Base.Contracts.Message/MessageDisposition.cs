namespace Base.Contracts.Message;

/// <summary>
/// Defines what a consumer tells the broker after processing a delivery.
/// </summary>
public enum MessageDisposition
{
    /// <summary>
    /// The message was handled (or deliberately dropped) and is removed from the queue.
    /// </summary>
    Ack,

    /// <summary>
    /// Processing failed transiently and the message is put back on the queue for another attempt. The queue's
    /// own delivery limit (managed externally) dead-letters it after repeated failures.
    /// </summary>
    Requeue,

    /// <summary>
    /// The message can never be processed and is rejected without requeue, so a dead-letter binding (managed
    /// externally) captures it.
    /// </summary>
    Reject
}
