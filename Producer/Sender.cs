using System.Text;
using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Producer.Contracts;
using Producer.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Producer;

internal class Sender : ISenderService, IDisposable
{
    private IModel? _channel;
    private IConnection? _connection;
    private bool _isConnected;
    private string? _replyQueueName;
    private readonly object _sync = new ();
    
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly IReceiveHandler _receiveHandler;
    private readonly ILogger _logger;

    public Sender(
        IOptions<RabbitMqOptions> options,
        IReceiveHandler receiveHandler,
        ILogger<Sender> logger)
    {
        _receiveHandler = receiveHandler;
        _logger = logger;
        _rabbitMqOptions = options.Value;
        if (_rabbitMqOptions.Host.Empty()||
            _rabbitMqOptions.QueueName.Empty())
        {
            throw new AggregateException("Please, set section RabbitMw int the file settings");
        }
    }


    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }

    public bool SendAction(string payload)
    {
        if (!_isConnected)
        {
            lock (_sync)
            {
                if (!Connect())
                {
                    return false;
                }
            }
        }
        try
        {
            var props = _channel!.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            var body = Encoding.UTF8.GetBytes(payload);
            _channel.BasicPublish(
                string.Empty,
                _rabbitMqOptions.QueueName,
                props,
                body);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e,ErrorFactory.GetRabbitMqError());
            return false;
        }
    }

    private bool Connect()
    {
        try
        {
            if (_isConnected)
            {
                return true;
            }
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.Host,
                UserName = _rabbitMqOptions.User,
                Password = _rabbitMqOptions.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            var args = new Dictionary<string, object> { { "x-message-ttl", _rabbitMqOptions.MessageExpireTime } };
            
            _channel.QueueDeclare(
                _rabbitMqOptions.QueueName,
                false,
                false,
                false,
                args);
            
            _replyQueueName = _channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                _receiveHandler.Processing(response);
            };
            _channel.BasicConsume(
                consumer: consumer,
                queue: _replyQueueName,
                autoAck: true);
            _isConnected = true;
            return true;
        }
        catch (Exception e)
        {
            _isConnected = false;
            _logger.LogError(e, "An error occurred while connecting to RabbitMq");
            
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
        
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }

            return false;
        }
    }
}