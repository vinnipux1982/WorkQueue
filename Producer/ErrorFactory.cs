namespace Producer;

internal static class ErrorFactory
{
    public static string GetRabbitMqError() => "A rabbit mq error";
    
    public static string GetExpireTimeError() => "Expiry time";
}