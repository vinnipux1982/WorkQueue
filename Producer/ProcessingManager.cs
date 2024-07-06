using Common;
using Consumer;
using Producer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Producer
{
    internal class ProcessingManager : IHandlerMsg
    {
        private readonly ActionsStorage _actionStorage;

        private readonly ActionsQueue _queue;

        private readonly ISenderService _senderService;

        public ProcessingManager(
            ActionsQueue queue,
            ISenderService senderService)
        {
            _actionStorage = new ActionsStorage();
            _queue = queue;

        }

        public async Task HandleInRequest()
        {
            while (true)
            {

                var actionInfo = _queue.Dequeue();

                var message = JsonSerializer.Serialize(new { ClienId = actionInfo.ClientId, Payload = actionInfo.Payload });
                await _senderService.SendAction(message);

                _actionStorage.Add(actionInfo);
            }
        }

        public void Processing(string message)
        {
            if (message.Empty())
            {
                return;
            }

            var reciveMsg = JsonSerializer.Deserialize<Message>(message);

            if (reciveMsg == null)
            {
                return;
            }

            var actionInfo = _actionStorage.GetActionInfo(reciveMsg.ClientId);
            if (actionInfo == null)
            {
                return;
            }

            actionInfo.TaskCompletionSource.SetResult(reciveMsg.Payload);

        }
    }

    internal record Message(Guid ClientId, string Payload);


}
