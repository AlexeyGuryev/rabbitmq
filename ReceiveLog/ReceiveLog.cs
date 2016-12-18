using System;
using System.Text;
using Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLog
{
    class ReceiveLog
    {
        static void Main(string[] args)
        {
            using (var connection = Connector.CreateConnectionFactory().CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, "logs", "");
                Console.WriteLine("Waiting for logs");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received message: {0}", message);
                };

                channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
