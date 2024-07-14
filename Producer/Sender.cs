
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;
using Common;

namespace Producer
{
    public class Sender : Contracts.ISenderService, IDisposable
    {

        private readonly IModel _channel;
        private readonly string _hostName;
        private readonly string _queueName;


        public Sender(string hostName, string queueName)
        {
            if (hostName.Empty())
            {
                throw new ArgumentNullException("hostName");
            }
            if (queueName.Empty())
            {
                throw new ArgumentNullException("queueName");
            }

            _hostName = hostName;
            _queueName = queueName;
            var factory = new ConnectionFactory { HostName = hostName };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

        }

        public void Dispose()
        {
            _channel.Dispose();
        }

        public async Task SendAction(string payload)
        {
            var body = Encoding.UTF8.GetBytes(payload);

            Console.WriteLine($"Send payload: {payload}");
            
            _channel.BasicPublish(exchange: string.Empty,
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
            
        }
    }
}
