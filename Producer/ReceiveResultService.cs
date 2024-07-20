using System.Text;
using System.Text.Json;
using Common;
using Producer.Contracts;
using Producer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Producer;

internal class ReceiveResultService :IReceiverService, IDisposable
{
    private IModel? _channel;
    private Action<string>? _handler;

    public void Dispose()
    {
        _channel?.Dispose();
    }

    private void ReceiveMsg(object? sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var userAction = JsonSerializer.Deserialize<UserAction>(message);
        if (userAction == null) return;
        _handler?.Invoke(message);
    }

    public void Init(string hostName, string queueName, Action<string> handler)
    {
        if (hostName.Empty()) throw new ArgumentNullException(nameof(hostName));
        if (queueName.Empty()) throw new ArgumentNullException(nameof(queueName));

        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        
        var factory = new ConnectionFactory { HostName = hostName };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.QueueDeclare(queueName,
            false,
            false,
            false,
            null);
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += ReceiveMsg;

        _channel.BasicConsume(queueName,
            true,
            consumer);
    }
}