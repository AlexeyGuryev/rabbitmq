using System;
using System.Text;
using Core;
using RabbitMQ.Client;

namespace Send
{
    class Send
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

                    var messageToSend = "Hello World!";
                    var bodyToSend = Encoding.UTF8.GetBytes(messageToSend);
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "hello",
                        basicProperties: null,
                        body: bodyToSend);

                    Console.WriteLine("Message sent: {0}", messageToSend);
                    Console.WriteLine("Press any key");
                    Console.ReadLine();
                }
            }
        }
    }
}
