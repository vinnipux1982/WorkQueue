using System.Text;
using Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer;

public class SyncConsumer : IDisposable
{
    private IModel? _receiveChannel;
    private IModel? _sendChannel;
    
    private readonly IHandlerMsg _handler;
    private readonly string _receiveHostName;
    private readonly string _receiveQueueName;
    
    private readonly string _sendHostName;
    private readonly string _sendQueueName;

    public SyncConsumer(
        string receiveHostName,
        string receiveQueueName,
        IHandlerMsg handler,
        string sendHostName,
        string sendQueueName)
    {
        if (receiveHostName.Empty()) 
            throw new ArgumentNullException(nameof(receiveHostName));
        
        if (receiveQueueName.Empty()) 
            throw new ArgumentNullException(nameof(receiveQueueName));

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));
        _handler = handler;
        _sendHostName = sendHostName;
        _sendQueueName = sendQueueName;
        _receiveHostName = receiveHostName;
        _receiveQueueName = receiveQueueName;   
        
        InitReceivedChannel();
        InitSendChannel();
        
    }

    public void Dispose()
    {
        _receiveChannel?.Dispose();
        _sendChannel?.Dispose();
    }

    private void InitReceivedChannel()
    {
        var factory = new ConnectionFactory { HostName = _receiveHostName };
        var connection = factory.CreateConnection();
        _receiveChannel = connection.CreateModel();

        _receiveChannel.QueueDeclare(
            _receiveQueueName,
            false,
            false,
            false,
            null);
        var consumer = new EventingBasicConsumer(_receiveChannel);
        consumer.Received += ReceiveMsg;

        _receiveChannel.BasicConsume(
            _receiveQueueName,
            true,
            consumer);
    }

    private void InitSendChannel()
    {
        var factory = new ConnectionFactory { HostName = _sendHostName };
        var connection = factory.CreateConnection();
        _sendChannel = connection.CreateModel();
        _sendChannel.QueueDeclare(
            _sendQueueName,
            false,
            false,
            false,
            null);
    }
    
    private void ReceiveMsg(object? sender, BasicDeliverEventArgs ea)
    {
        Console.WriteLine("Start processing");
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var result = _handler.Processing(message);
        Console.WriteLine($" [x] Received {message}");
        SendAction(result);
    }
    
    private void SendAction(string payload)
    {
        var body = Encoding.UTF8.GetBytes(payload);

        Console.WriteLine($"Send payload: {payload}");

        _sendChannel.BasicPublish(string.Empty,
            _sendQueueName,
            null,
            body);
    }
}