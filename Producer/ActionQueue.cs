using System.Collections.Concurrent;
using Producer.DTOs;

namespace Producer;

internal class ActionsQueue
{
    private readonly BlockingCollection<ActionInfo> _queue = [];

    public void Enqueue(ActionInfo action)
    {
        _queue.Add(action);
    }

    public ActionInfo Dequeue(CancellationToken stoppingToken)
    {
        return _queue.Take(stoppingToken);
    }

    public void Clear()
    {
        _queue.CompleteAdding();
        while (_queue.TryTake(out var actionInfo))
        {
            actionInfo.TaskCompletionSource.SetCanceled();
        }
    }
}