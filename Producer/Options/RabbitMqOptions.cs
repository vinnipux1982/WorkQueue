namespace Producer.Options;

internal class RabbitMqOptions
{
    public string Host { get; set; } = "127.0.0.1";

    public string QueueName { get; set; } = "Test";

    public int MessageExpireTime { get; set; } = 180000;

    public string User { get; set; } = "guest";

    public string Password { get; set; } = "guest";
}