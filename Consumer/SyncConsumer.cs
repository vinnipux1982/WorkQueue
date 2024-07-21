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
    

    public SyncConsumer(
        string receiveHostName,
        string receiveQueueName,
        IHandlerMsg handler)
    {
        if (receiveHostName.Empty()) 
            throw new ArgumentNullException(nameof(receiveHostName));
        
        if (receiveQueueName.Empty()) 
            throw new ArgumentNullException(nameof(receiveQueueName));

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));
        _handler = handler;
        _receiveHostName = receiveHostName;
        _receiveQueueName = receiveQueueName; 
        
        InitReceivedChannel();
        
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
        _receiveChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var consumer = new EventingBasicConsumer(_receiveChannel);
        consumer.Received += ReceiveMsg;

        _receiveChannel.BasicConsume(
            _receiveQueueName,
            false,
            consumer);
    }
    
    private void ReceiveMsg(object? sender, BasicDeliverEventArgs ea)
    {
        Console.WriteLine("Start processing");
        var props = ea.BasicProperties;
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        var result = _handler.Processing(message);
        Console.WriteLine($" [x] Received {message}");
        
        var responseBytes = Encoding.UTF8.GetBytes(result);
        
        _receiveChannel.BasicPublish(exchange: string.Empty,
            routingKey: props.ReplyTo,
            null,
            body: responseBytes);
        
        _receiveChannel!.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
    
    
}