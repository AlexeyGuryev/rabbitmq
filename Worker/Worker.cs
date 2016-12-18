using System;
using System.Text;
using System.Threading;
using Core;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace Worker
{
    class Worker
    {
        private const string QUEUE_NAME = "task_queue";

        static void Main(string[] args)
        {
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


                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, eventArgs) =>
                    {
                        var body = eventArgs.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Message received: {0}", message);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(1000 * dots);
                        Console.WriteLine("Done");

                        channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: QUEUE_NAME, noAck: false, consumer: consumer);
                    Console.WriteLine("Press any key");
                    Console.ReadLine();
                }
            }
        }
    }
}
