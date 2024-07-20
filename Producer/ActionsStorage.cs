using System.Collections.Concurrent;
using Producer.Models;

namespace Producer;

internal class ActionsStorage
{
    private readonly ConcurrentDictionary<Guid, ActionInfo> _actions = [];

    public void Add(ActionInfo action)
    {
        _actions.TryAdd(action.ClientId, action);
    }

    public ActionInfo? GetActionInfo(Guid requestId)
    {
        if (_actions.TryGetValue(requestId, out var actionInfo)) return actionInfo;
        return null;
    }
}