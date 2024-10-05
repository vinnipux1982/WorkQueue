namespace Producer.Contracts;

public interface IRabbitMqSender
{
    Task<string?> SendActionAsync(string payload);
}