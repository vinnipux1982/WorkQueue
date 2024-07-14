using System.Collections.Concurrent;
using Producer.Contracts;
using Producer.Models;

namespace Producer;

internal class BackgroundWorker(ActionsQueue actionsQueue) : IWorker, IDisposable, IReceiverService
{
    private readonly ConcurrentDictionary<Guid, ActionInfo> _actions = [];
    private bool _disposed;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void ProcessingResult(UserAction userActionResult)
    {
        if (_actions.Remove(userActionResult.ClientId, out var actionInfo))
            actionInfo.TaskCompletionSource.TrySetResult(userActionResult.Payload);
    }

    public async Task SendActionAsync(string payload)
    {
        var completionSource = new TaskCompletionSource<string?>();
        var clientInfo = new ActionInfo(completionSource, payload, Guid.NewGuid());
        actionsQueue.Enqueue(clientInfo);
        _actions.TryAdd(clientInfo.ClientId, clientInfo);

        await completionSource.Task;
    }
}