using System.Text.Json;
using Common;
using Producer.Contracts;

namespace Producer;

internal class ProcessingManager(
    ActionsQueue queue,
    ISenderService senderService) : IDisposable
{
    private readonly ActionsStorage _actionStorage = new();

    private bool _isRun = true;

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
        Task.Run(()=> HandleInRequest(token), token);
    }

    public void Stop()
    {
        _isRun = false;
    }

    private async Task HandleInRequest(CancellationToken token)
    {
        while (_isRun)
        {
            var actionInfo = queue.Dequeue();
            token.ThrowIfCancellationRequested();

            var message = JsonSerializer.Serialize(new Message(actionInfo.ClientId, actionInfo.Payload));
            await senderService.SendAction(message);

            _actionStorage.Add(actionInfo);
        }
    }

    public void Dispose()
    {
        Stop();
        ((IDisposable)senderService).Dispose();
    }
}

internal record Message(Guid ClientId, string Payload);