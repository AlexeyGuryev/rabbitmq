using RabbitMQ.Client;

namespace Core
{
    public static class Connector
    {
        public static ConnectionFactory CreateConnectionFactory()
        {
            return new ConnectionFactory()
            {
                Uri = @"amqp://apdkupga:Ocf8DkD9KTHSIXU9rXi7PjB0WH-EzIsB@spotted-monkey.rmq.cloudamqp.com/apdkupga",
                Password = @"Ocf8DkD9KTHSIXU9rXi7PjB0WH-EzIsB",
                UserName = "apdkupga"
            };
        }
    }
}
