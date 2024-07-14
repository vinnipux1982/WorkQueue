namespace Producer.Contracts;

public interface IWorker
{
    Task SendActionAsync(string payload);
}