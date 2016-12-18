using System;
using System.Text;
using Core;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace Receive
{
    class Receive
    {
        static void Main(string[] args)
        {
            using (var connection = Connector.CreateConnectionFactory().CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "hello",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);


                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, eventArgs) =>
                    {
                        var body = eventArgs.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Message received: {0}", message);
                    };
                    channel.BasicConsume("hello", noAck: true, consumer: consumer);
                    Console.WriteLine("Press any key");
                    Console.ReadLine();
                }
            }
        }
    }
}
