namespace Producer.Contracts;

public interface ISenderService
{
    Task SendAction(string payload);
}