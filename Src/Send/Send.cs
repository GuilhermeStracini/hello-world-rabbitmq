using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Send;

class Send
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using (var connection = await factory.CreateConnectionAsync())
        await using (var channel = await connection.CreateChannelAsync())
        {
            await channel.QueueDeclareAsync(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "hello",
                basicProperties: new BasicProperties(),
                mandatory: true,
                body: body
            );
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
