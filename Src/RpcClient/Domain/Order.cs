using System;

namespace RpcClient.Domain;

/// <summary>
/// Class Order.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    /// <value>The amount.</value>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    /// <value>The status.</value>
    public string Status => OrderStatus.ToString();

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    /// <value>The order status.</value>
    private OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class.
    /// </summary>
    /// <param name="amount">The amount.</param>
    public Order(decimal amount)
    {
        Id = DateTime.Now.Ticks;
        OrderStatus = OrderStatus.Processing;
        Amount = amount;
    }
}
