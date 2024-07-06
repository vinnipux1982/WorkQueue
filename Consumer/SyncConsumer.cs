using RabbitMQ.Client;
using Common;
using RabbitMQ.Client.Events;
using System.Threading.Channels;
using System.Text;
using System.Reflection.Metadata;

namespace Consumer
{
    public class SyncConsumer: IDisposable
    {

        private readonly IModel _channel;
        private readonly string _hostName;
        private readonly string _queueName;
        private readonly IHandlerMsg _handler;

        public SyncConsumer(string hostName, string queueName, IHandlerMsg handler)
        {
            if (hostName.Empty())
            {
                throw new ArgumentNullException("hostName");
            }
            if (queueName.Empty())
            {
                throw new ArgumentNullException("queueName");
            }

            if (handler == null)
            {
                throw new ArgumentNullException("queueName");
            }
            _handler = handler;
            _hostName = hostName;
            _queueName = queueName;
            var factory = new ConnectionFactory { HostName = hostName };
            using var connection = factory.CreateConnection();
            var _channel = connection.CreateModel();

            _channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ReciveMsg;
            
            _channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void ReciveMsg(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _handler.Processing(message);
            Console.WriteLine($" [x] Received {message}");
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}
