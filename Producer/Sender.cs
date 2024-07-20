using System.Text;
using Common;
using Producer.Contracts;
using RabbitMQ.Client;

namespace Producer;

internal class Sender : ISenderService, IDisposable
{
    private readonly IModel _channel;
    private readonly string _queueName;


    public Sender(string hostName, string queueName)
    {
        if (hostName.Empty()) throw new ArgumentNullException(nameof(hostName));
        if (queueName.Empty()) throw new ArgumentNullException(nameof(queueName));

        _queueName = queueName;
        var factory = new ConnectionFactory { HostName = hostName };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queueName,
            false,
            false,
            false,
            null);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }

    public async Task SendAction(string payload)
    {
        var body = Encoding.UTF8.GetBytes(payload);
        _channel.BasicPublish(string.Empty,
            _queueName,
            null,
            body);
    }
}