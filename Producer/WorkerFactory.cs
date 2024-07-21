using Common;
using Producer.Contracts;

namespace Producer;

public class WorkerFactory:IDisposable
{
    private static IWorker? _worker;
    private static ActionsQueue? _actionQueue;
    private static ProcessingManager? _processingManager;
    private static IReceiverService? _receiveResultService;
    private static string? _hostName;
    private static string? _outQueue;

    public WorkerFactory(string hostName, string? outQueueName = default)
    {
        if (outQueueName!.Empty()) outQueueName = "OutTaskQueue";
        
        if (hostName.Empty()) hostName = "127.0.0.1";

        _hostName = hostName;
        _outQueue = outQueueName!;
    }
    
    public IWorker GetWorker()
    {
        if (_worker!=null)
        {
            return _worker;
        }
        
        _actionQueue = new ActionsQueue();
        _worker = new BackgroundWorker(_actionQueue);
        _processingManager = new ProcessingManager(_actionQueue);
        var sendService = new Sender(_hostName!, _outQueue!, _processingManager.Processing);
        _processingManager.Start(sendService);
        
        return _worker;
    }
    public void Dispose()
    {
        _processingManager?.Stop();
        ((IDisposable)_receiveResultService!).Dispose();
    }
}