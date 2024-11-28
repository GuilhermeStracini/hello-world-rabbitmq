using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace NewTask;

class NewTask
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using (var connection = await factory.CreateConnectionAsync())
        await using (var channel = await connection.CreateChannelAsync())
        {
            await channel.QueueDeclareAsync(
                queue: "task_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties();
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "task_queue",
                mandatory: true,
                basicProperties: properties,
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
