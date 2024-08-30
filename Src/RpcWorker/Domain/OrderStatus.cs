namespace RpcWorker.Domain;

/// <summary>
/// Enum OrderStatus
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// The processing
    /// </summary>
    Processing = 0,

    /// <summary>
    /// The approved
    /// </summary>
    Approved,

    /// <summary>
    /// The declined
    /// </summary>
    Declined,
}
