using System.Text;
using System.Text.Json;
using Common;
using Producer.Contracts;
using Producer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Producer;

internal class ReceiveResultService : IDisposable
{
    private readonly IModel _channel;
    private readonly IReceiverService _handler;
    private readonly string _hostName;
    private readonly string _queueName;


    public ReceiveResultService(IReceiverService handler, string hostName, string queueName)
    {
        if (hostName.Empty()) throw new ArgumentNullException("hostName");
        if (queueName.Empty()) throw new ArgumentNullException("queueName");

        if (handler == null) throw new ArgumentNullException("queueName");
        _handler = handler;
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
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += ReceiveMsg;

        _channel.BasicConsume(queueName,
            true,
            consumer);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }

    private void ReceiveMsg(object? sender, BasicDeliverEventArgs ea)
    {
        Console.WriteLine("Start processing");
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var userAction = JsonSerializer.Deserialize<UserAction>(message);

        if (userAction == null) return;

        _handler.ProcessingResult(userAction);
        Console.WriteLine($" [x] Received {message}");
    }
}