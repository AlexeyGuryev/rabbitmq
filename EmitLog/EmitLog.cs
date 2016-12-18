using System;
using System.Text;
using Core;
using RabbitMQ.Client;

namespace EmitLog
{
    class EmitLog
    {
        static void Main1(string[] args)
        {
            using (var connection = Connector.CreateConnectionFactory().CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("logs", "fanout");
                var message = Util.GetMessageFromArgs(args);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);
                Console.WriteLine("Message sent: {0}", message);
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
