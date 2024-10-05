using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Producer.Contracts;
using Producer.DTOs;
using Producer.Options;

namespace Producer;

internal class ActionsStorage : IActionStorage, IDisposable
{
    
    private readonly ConcurrentDictionary<Guid, ActionInfo> _actions = [];
    private readonly Timer _timer;
    private readonly int _expireTimeMs; 
    
    public ActionsStorage(IOptions<RabbitMqOptions> options)
    {
        _expireTimeMs = options.Value.MessageExpireTime + 1000;
        _timer = new Timer(ClearExpireActions, null, _expireTimeMs / 2, Timeout.Infinite);
    }

    public void Add(ActionInfo action)
    {
        _actions.TryAdd(action.ClientId, action);
    }

    public ActionInfo? GetAction(Guid requestId)
    {
        return _actions.TryRemove(requestId, out var actionInfo) ? actionInfo : null;
    }

    public void Dispose()
    {
        try
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            foreach (var actionInfo in _actions)
            {
                actionInfo.Value.TaskCompletionSource.SetCanceled();
            }

            _actions.Clear();
            _timer.Dispose();
        }
        catch
        {
            // ignored
        }
    }

    private void ClearExpireActions(object? state)
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        var removedDate = DateTimeOffset.UtcNow.AddSeconds(-_expireTimeMs);
        var actions = _actions.Values.ToList();
        
        foreach (var action in actions.Where(action => action.StartDate <= removedDate))
        {
            _actions.TryRemove(action.ClientId, out _);
            action.TaskCompletionSource.SetResult(ErrorFactory.GetRabbitMqError());
        }

        _timer.Change(_expireTimeMs / 2, Timeout.Infinite);
    }
}