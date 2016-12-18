using System;
using System.Text;
using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLog
{
    class ReceiveLogDirect
    {
        static void Main(string[] args)
        {
            using (var connection = Connector.CreateConnectionFactory().CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");
                var queueName = channel.QueueDeclare().QueueName;

                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
                    Environment.ExitCode = 1;
                    return;
                }

                foreach (var severity in args)
                {
                    channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: severity);
                }

                Console.WriteLine("Waiting for logs");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = eventArgs.RoutingKey;
                    Console.WriteLine("Received message with severity {0}: {1}", routingKey, message);
                };

                channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
