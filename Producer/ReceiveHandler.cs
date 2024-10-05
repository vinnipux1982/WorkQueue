using System.Text.Json;
using Common;
using Producer.Contracts;
using Producer.DTOs;

namespace Producer;

internal class ReceiveHandler(IActionStorage actionStorage) : IReceiveHandler
{
    public void Processing(string message)
    {
        if (message.Empty()) return;

        var receiveMsg = JsonSerializer.Deserialize<Message>(message);

        if (receiveMsg == null) return;

        var actionInfo = actionStorage.GetAction(receiveMsg.ClientId);

        actionInfo?.TaskCompletionSource.SetResult(receiveMsg.Payload);
    }
}