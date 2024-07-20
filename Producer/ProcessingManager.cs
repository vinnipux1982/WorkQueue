using System.Text.Json;
using Common;
using Producer.Contracts;

namespace Producer;

internal class ProcessingManager
{
    private readonly ActionsStorage _actionStorage;

    private readonly ActionsQueue _queue;

    private readonly ISenderService _senderService;

    private bool _isRun = true;

    public ProcessingManager(
        ActionsQueue queue,
        ISenderService senderService)
    {
        _actionStorage = new ActionsStorage();
        _queue = queue;
        _senderService = senderService;
    }

    public void Processing(string message)
    {
        if (message.Empty()) return;

        var reciveMsg = JsonSerializer.Deserialize<Message>(message);

        if (reciveMsg == null) return;

        var actionInfo = _actionStorage.GetActionInfo(reciveMsg.ClientId);
        if (actionInfo == null) return;

        actionInfo.TaskCompletionSource.SetResult(reciveMsg.Payload);
    }

    public void Start(CancellationToken token = default)
    {
        Task.Run(() => HandleInRequest(), token);
    }

    public void Stop()
    {
        _isRun = false;
    }

    public async Task HandleInRequest()
    {
        while (_isRun)
        {
            var actionInfo = _queue.Dequeue();

            var message = JsonSerializer.Serialize(new Message(actionInfo.ClientId, actionInfo.Payload));
            await _senderService.SendAction(message);

            _actionStorage.Add(actionInfo);
        }
    }
}

internal record Message(Guid ClientId, string Payload);