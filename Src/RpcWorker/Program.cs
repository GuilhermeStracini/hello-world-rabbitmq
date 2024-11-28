using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RpcWorker.Domain;
using RpcWorker.Services;

namespace RpcWorker;

/// <summary>
/// Class Program.
/// </summary>
class Program
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await Consumer(channel);

        Console.ReadLine();
    }

    /// <summary>
    /// Consumers the specified channel.
    /// </summary>
    /// <param name="channel">The channel.</param>
    private static async Task Consumer(IChannel channel)
    {
        var consumer = await InitializerConsumer(channel, nameof(Order));

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var incomingMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"{DateTime.Now:o} Incoming => {incomingMessage}");

                var order = JsonSerializer.Deserialize<Order>(incomingMessage);
                order.SetStatus(ProcessOrderStatus(order.Amount));

                var replyMessage = JsonSerializer.Serialize(order);
                Console.WriteLine($"{DateTime.Now:o} Reply => {replyMessage}");

                await SendReplyMessage(replyMessage, channel, ea);
            }
            catch
            {
                Console.WriteLine($"Error {ea.DeliveryTag}");
            }
        };
    }

    /// <summary>
    /// Processes the order status.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <returns>OrderStatus.</returns>
    private static OrderStatus ProcessOrderStatus(decimal amount)
    {
        return OrderServices.OnStore(amount);
    }

    /// <summary>
    /// Sends the reply message.
    /// </summary>
    /// <param name="replyMessage">The reply message.</param>
    /// <param name="channel">The channel.</param>
    /// <param name="ea">The <see cref="BasicDeliverEventArgs" /> instance containing the event data.</param>
    private static async Task SendReplyMessage(
        string replyMessage,
        IChannel channel,
        BasicDeliverEventArgs ea
    )
    {
        var props = ea.BasicProperties;
        var replyProps = new BasicProperties { CorrelationId = props.CorrelationId };

        var responseBytes = Encoding.UTF8.GetBytes(replyMessage);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: props.ReplyTo,
            basicProperties: replyProps,
            mandatory: true,
            body: responseBytes
        );

        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
    }

    /// <summary>
    /// Initializers the consumer.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <returns>EventingBasicConsumer.</returns>
    private static async Task<AsyncEventingBasicConsumer> InitializerConsumer(
        IChannel channel,
        string queueName
    )
    {
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await channel.BasicQosAsync(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        return consumer;
    }
}
