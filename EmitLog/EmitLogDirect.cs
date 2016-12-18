using System;
using System.Linq;
using System.Text;
using Core;
using RabbitMQ.Client;

namespace EmitLog
{
    class EmitLogDirect
    {
        static void Main(string[] args)
        {
            using (var connection = Connector.CreateConnectionFactory().CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                var severity = args.Length > 0
                    ? args[0]
                    : "info";

                var message = args.Length > 1
                    ? string.Join(" ", args.Skip(1))
                    : "Hello world!";

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "direct_logs", routingKey: severity, basicProperties: null, body: body);
                Console.WriteLine("Message sent: {0} with severity: {1}", message, severity);
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
