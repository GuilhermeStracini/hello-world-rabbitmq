// ***********************************************************************
// Assembly         : RpcClient
// Author           : Guilherme Branco Stracini
// Created          : 07-23-2020
//
// Last Modified By : Guilherme Branco Stracini
// Last Modified On : 07-23-2020
// ***********************************************************************
// <copyright file="Program.cs" company="Guilherme Branco Stracini ME">
//     Copyright (c) Guilherme Branco Stracini ME. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RpcClient.Domain;
using System;
using System.Text;
using System.Text.Json;

namespace RpcClient
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        private static void Main()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var replyQueue = $"{nameof(Order)}_return";
            var correlationId = Guid.NewGuid().ToString();

            channel.QueueDeclare(queue: replyQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: nameof(Order), durable: false, exclusive: false, autoDelete: false, arguments: null);

            Consumer(channel, correlationId, replyQueue);

            Publisher(channel, correlationId, replyQueue);
        }

        /// <summary>
        /// Consumers the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="replyQueue">The reply queue.</param>
        private static void Consumer(IModel channel, string correlationId, string replyQueue)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                if (correlationId == ea.BasicProperties.CorrelationId)
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received: {message}");

                    return;
                }

                Console.WriteLine(
                    $"Discarded message, invalid correlation identifiers. Original: {correlationId} | Received: {ea.BasicProperties.CorrelationId}");
            };

            channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);
        }

        /// <summary>
        /// Publishers the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="replyQueue">The reply queue.</param>
        private static void Publisher(IModel channel, string correlationId, string replyQueue)
        {
            var props = channel.CreateBasicProperties();

            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueue;

            while (true)
            {
                var body = GetOrder();

                if (body == null)
                    continue;

                channel.BasicPublish(exchange: "", routingKey: nameof(Order), basicProperties: props, body: body);

                Console.WriteLine($"Published\r\n");

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
                return null;

            var amount = decimal.Parse(line);

            var order = new Order(amount);
            var message = JsonSerializer.Serialize(order);
            return Encoding.UTF8.GetBytes(message);

        }
    }
}
