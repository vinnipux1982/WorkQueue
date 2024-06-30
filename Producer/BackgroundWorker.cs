using Producer.Models;
using System.Collections.Concurrent;

namespace Producer
{
    internal class BackgroundWorker : IDisposable
    {
        private bool _disposed;

        private BlockingCollection<Action> _actions = [];

        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
        
        private readonly TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

        private readonly ActionsQueue _actionsQueue = new ActionsQueue();

        

        public async Task SendActionAsync(string payload)
        {
            var completionSource = new TaskCompletionSource<string?>();
            var clientInfo = new ActionInfo(completionSource, payload, Guid.NewGuid());
            _actionsQueue.Add(clientInfo);



        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
