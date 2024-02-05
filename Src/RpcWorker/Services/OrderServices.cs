using RpcWorker.Domain;

namespace RpcWorker.Services;

/// <summary>
/// Class OrderServices. This class cannot be inherited.
/// </summary>
public sealed class OrderServices
{
    /// <summary>
    /// Called when [store].
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <returns>OrderStatus.</returns>
    public static OrderStatus OnStore(decimal amount)
    {
        return (amount < 0 || amount > 10000) ? OrderStatus.Declined : OrderStatus.Approved;
    }
}
