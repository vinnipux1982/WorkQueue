using Producer.Contracts;
using Producer.DTOs;

namespace Producer;

internal class RabbitMqSender(ActionsQueue actionsQueue) : IRabbitMqSender
{
    public Task<string?> SendActionAsync(string payload)
    {
        var completionSource = new TaskCompletionSource<string?>();
        var clientInfo = new ActionInfo(completionSource, payload, Guid.NewGuid(),DateTimeOffset.UtcNow);
        actionsQueue.Enqueue(clientInfo);
        return completionSource.Task;
    }
}