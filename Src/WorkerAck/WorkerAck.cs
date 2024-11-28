using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerAck;

static class WorkerAck
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: "task_queue_ack",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine(" [x] Received {0}", message);

            int dots = message.Split('.').Length - 1;
            Thread.Sleep(dots * 1000);

            Console.WriteLine(" [x] Done");

            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        await channel.BasicConsumeAsync(
            queue: "task_queue_ack",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
