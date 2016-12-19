using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using RabbitMQ.Client;

namespace RPCClient
{
    public class RPCClient
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private QueueingBasicConsumer consumer;

        public RPCClient()
        {
            connection = Connector.CreateConnectionFactory().CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue: replyQueueName, noAck: true, consumer: consumer);
        }

        public string Call(string message)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrId;

            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: messageBytes);

            while (true)
            {
                var eventArgs = consumer.Queue.Dequeue();
                if (eventArgs.BasicProperties.CorrelationId == corrId)
                {
                    return Encoding.UTF8.GetString(eventArgs.Body);
                }
            }
        }

        public void Close()
        {
            connection.Close();
        }
    }


    class RPC
    {
        static void Main()
        {
            var rpcClient = new RPCClient();
            Console.WriteLine("Requesting fib(30)");
            var response = rpcClient.Call("30");
            Console.WriteLine("Got: {0}", response);
            rpcClient.Close();
        }
    }
}
