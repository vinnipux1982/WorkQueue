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

        private readonly ActionsStorage _actionsQueue = new ActionsStorage();

        

        public async Task SendActionAsync(string payload)
        {
            var completionSource = new TaskCompletionSource<string?>();
            var clientInfo = new ActionInfo(completionSource, payload, Guid.NewGuid());
            _actionsQueue.Add(clientInfo);


            await completionSource.Task;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
