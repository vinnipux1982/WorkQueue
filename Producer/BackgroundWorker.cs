using Producer.Contracts;
using Producer.Models;
using System.Collections.Concurrent;

namespace Producer
{
    internal class BackgroundWorker : IWorker, IDisposable
    {
        private bool _disposed;

        private BlockingCollection<Action> _actions = [];

        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
        
        private readonly TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

        private readonly ActionsQueue _actionsQueue;

        public BackgroundWorker(ActionsQueue actionsQueue)
        {
            _actionsQueue = actionsQueue;
        }

        public async Task SendActionAsync(string payload)
        {
            var completionSource = new TaskCompletionSource<string?>();
            var clientInfo = new ActionInfo(completionSource, payload, Guid.NewGuid());
            _actionsQueue.Enqueue(clientInfo);

            await completionSource.Task;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
