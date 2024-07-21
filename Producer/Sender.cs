using System.Text;
using Common;
using Producer.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Producer;

internal class Sender : ISenderService, IDisposable
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _queueName;
    private readonly string _replyQueueName;


    public Sender(string hostName, string queueName, Action<string> receiveHandler)
    {
        if (hostName.Empty()) throw new ArgumentNullException(nameof(hostName));
        if (queueName.Empty()) throw new ArgumentNullException(nameof(queueName));

        _queueName = queueName;
        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _replyQueueName = _channel.QueueDeclare().QueueName;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            receiveHandler?.Invoke(response);
        };

        _channel.BasicConsume(consumer: consumer,
            queue: _replyQueueName,
            autoAck: true);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }

    public async Task SendAction(string payload)
    {
        IBasicProperties props = _channel.CreateBasicProperties();
        props.ReplyTo = _replyQueueName;
        var body = Encoding.UTF8.GetBytes(payload);
        _channel.BasicPublish(
            string.Empty,
            _queueName,
            props,
            body);
    }
}