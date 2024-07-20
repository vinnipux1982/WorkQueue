using Common;
using Producer.Contracts;

namespace Producer;

public class ProducerFactory
{
    private static IWorker _worker;
    private static ActionsQueue _actionQueue;
    private static ProcessingManager _processingManager;
    private static ReceiveResultService _receiveResultService;

    public static IWorker GetWorker(string hostName, string? outQueueName = default, string? inQueueName = default)
    {
        if (outQueueName!.Empty()) outQueueName = "OutTaskQueue";

        if (inQueueName!.Empty()) inQueueName = "InResultQueue";
        _actionQueue = new ActionsQueue();
        _worker = new BackgroundWorker(_actionQueue);
        _processingManager = new ProcessingManager(
            _actionQueue,
            new Sender(hostName, outQueueName!));
        _processingManager.Start();

        _receiveResultService = new ReceiveResultService();
        _receiveResultService.Init(
            hostName,
            inQueueName!,
            _processingManager.Processing);

        return _worker;
    }

    public static void Stop()
    {
        _processingManager.Stop();
    }
}