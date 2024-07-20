using System.Collections.Concurrent;
using Producer.Models;

namespace Producer;

internal class ActionsQueue
{
    private readonly BlockingCollection<ActionInfo> _queue = [];

    public void Enqueue(ActionInfo action)
    {
        _queue.Add(action);
    }

    public ActionInfo Dequeue()
    {
        return _queue.Take();
    }
}