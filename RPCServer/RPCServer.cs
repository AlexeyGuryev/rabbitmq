using System;
using System.Text;
using Core;
using RabbitMQ.Client;

namespace RPCServer
{
    class RPCServer
    {
        static void Main(string[] args)
        {
            using (var connection = Connector.CreateConnectionFactory().CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "rpc_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicQos(0, 1, global: false);
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue", noAck: false, consumer: consumer);
                Console.WriteLine("Awaiting RPC requests");

                while (true)
                {
                    string response = string.Empty;
                    var eventArgs = consumer.Queue.Dequeue();
                    var body = eventArgs.Body;
                    var props = eventArgs.BasicProperties;

                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    var message = Encoding.UTF8.GetString(body);
                    int n = 0;
                    if (int.TryParse(message, out n))
                    {
                        Console.WriteLine("[.] fib({0})", message);
                        response = Fib(n).ToString();
                    }

                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                }
            }
        }

        private static int Fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return Fib(n - 1) + Fib(n - 2);
        }
    }
}
