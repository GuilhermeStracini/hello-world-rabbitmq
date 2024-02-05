using System;
using System.Text;
using System.Text.Json;
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
    static void Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        Consumer(channel);

        Console.ReadLine();
    }

    /// <summary>
    /// Consumers the specified channel.
    /// </summary>
    /// <param name="channel">The channel.</param>
    private static void Consumer(IModel channel)
    {
        var consumer = InitializerConsumer(channel, nameof(Order));

        consumer.Received += (_, ea) =>
        {
            try
            {
                var incomingMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"{DateTime.Now:o} Incoming => {incomingMessage}");

                var order = JsonSerializer.Deserialize<Order>(incomingMessage);
                order.SetStatus(ProcessOrderStatus(order.Amount));

                var replyMessage = JsonSerializer.Serialize(order);
                Console.WriteLine($"{DateTime.Now:o} Reply => {replyMessage}");

                SendReplyMessage(replyMessage, channel, ea);
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
    private static void SendReplyMessage(
        string replyMessage,
        IModel channel,
        BasicDeliverEventArgs ea
    )
    {
        var props = ea.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        var responseBytes = Encoding.UTF8.GetBytes(replyMessage);

        channel.BasicPublish(
            exchange: "",
            routingKey: props.ReplyTo,
            basicProperties: replyProps,
            body: responseBytes
        );

        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }

    /// <summary>
    /// Initializers the consumer.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <returns>EventingBasicConsumer.</returns>
    private static EventingBasicConsumer InitializerConsumer(IModel channel, string queueName)
    {
        channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

        return consumer;
    }
}
