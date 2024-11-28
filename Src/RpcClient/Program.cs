using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RpcClient.Domain;

namespace RpcClient;

/// <summary>
/// Class Program.
/// </summary>
static class Program
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var replyQueue = $"{nameof(Order)}_return";
        var correlationId = Guid.NewGuid().ToString();

        await channel.QueueDeclareAsync(
            queue: replyQueue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        await channel.QueueDeclareAsync(
            queue: nameof(Order),
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await Consumer(channel, correlationId, replyQueue);

        await Publisher(channel, correlationId, replyQueue);
    }

    /// <summary>
    /// Consumers the specified channel.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <param name="replyQueue">The reply queue.</param>
    private static async Task Consumer(IChannel channel, string correlationId, string replyQueue)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            if (correlationId == ea.BasicProperties.CorrelationId)
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");

                return Task.CompletedTask;
            }

            Console.WriteLine(
                $"Discarded message, invalid correlation identifiers. Original: {correlationId} | Received: {ea.BasicProperties.CorrelationId}"
            );

            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue: replyQueue, autoAck: true, consumer: consumer);
    }

    /// <summary>
    /// Publishers the specified channel.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <param name="replyQueue">The reply queue.</param>
    private static async Task Publisher(IChannel channel, string correlationId, string replyQueue)
    {
        var props = new BasicProperties { CorrelationId = correlationId, ReplyTo = replyQueue };

        while (true)
        {
            var body = GetOrder();

            if (body == null)
                continue;

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: nameof(Order),
                basicProperties: props,
                mandatory: true,
                body: body
            );

            Console.WriteLine("Published\r\n");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.C)
                break;

            Console.Clear();
        }
    }

    /// <summary>
    /// Gets the order.
    /// </summary>
    /// <returns>System.Byte[].</returns>
    private static byte[] GetOrder()
    {
        Console.Write("Informe o valor do pedido:");

        var line = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(line))
        {
            return [];
        }

        var amount = decimal.Parse(line);

        var order = new Order(amount);
        var message = JsonSerializer.Serialize(order);
        return Encoding.UTF8.GetBytes(message);
    }
}
