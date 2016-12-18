using System;
using System.Text;
using Core;
using RabbitMQ.Client;

namespace NewTask
{
    class NewTask
    {
        private const string QUEUE_NAME = "task_queue";

        static void Main(string[] args)
        {
            string message = Util.GetMessageFromArgs(args);
            var body = Encoding.UTF8.GetBytes(message);

            using (var connection = Connector.CreateConnectionFactory().CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: QUEUE_NAME,
                        basicProperties: properties,
                        body: body);

                    Console.WriteLine("Message sent: {0}", message);
                }
            }
        }
    }
}
