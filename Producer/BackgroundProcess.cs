using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Producer.Contracts;

namespace Producer;

internal class BackgroundProcess(
    ActionsQueue queue,
    ISenderService senderService,
    IActionStorage actionStorage,
    ILogger<BackgroundProcess> logger) : BackgroundService
{
    private CancellationToken  _stoppingToken;
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        try
        {
            return Task.Factory.StartNew(
                (_) =>
                {
                    try
                    {
                        SendingMsg();
                    }
                    catch (OperationCanceledException)
                    {
                        logger.LogInformation("Cancelled by the user");
                        throw;
                    }
                    catch (Exception e)
                    {
                        logger.LogInformation(e, "Emergency shutdown of the background process");
                    }
                },
                stoppingToken,
                TaskCreationOptions.LongRunning);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            queue.Clear();
        }
    }

    private void SendingMsg()
    {
        while (!_stoppingToken.IsCancellationRequested)
        {
            var action = queue.Dequeue(_stoppingToken);
            _stoppingToken.ThrowIfCancellationRequested();
            if (senderService.SendAction(action.Payload))
            {
                actionStorage.Add(action);
            }
            else
            {
                action.TaskCompletionSource.SetResult(ErrorFactory.GetRabbitMqError());
            }
        }
    }
    
}