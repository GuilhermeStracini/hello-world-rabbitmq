using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace NewTaskAck;

class NewTaskAck
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using (var connection = await factory.CreateConnectionAsync())
        await using (var channel = await connection.CreateChannelAsync())
        {
            await channel.QueueDeclareAsync(
                queue: "task_queue_ack",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "task_queue_ack",
                basicProperties: properties,
                mandatory: true,
                body: body
            );
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}
