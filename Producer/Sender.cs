using System.Text;
using Common;
using Producer.Contracts;
using RabbitMQ.Client;

namespace Producer;

public class Sender : ISenderService, IDisposable
{
    private readonly IModel _channel;
    private readonly string _hostName;
    private readonly string _queueName;


    public Sender(string hostName, string queueName)
    {
        if (hostName.Empty()) throw new ArgumentNullException("hostName");
        if (queueName.Empty()) throw new ArgumentNullException("queueName");

        _hostName = hostName;
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

        Console.WriteLine($"Send payload: {payload}");

        _channel.BasicPublish(string.Empty,
            _queueName,
            null,
            body);
    }
}