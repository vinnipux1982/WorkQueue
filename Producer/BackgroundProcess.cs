using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Producer.Contracts;
using Producer.DTOs;

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
                    finally
                    {
                        queue.Clear();
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
    }

    private void SendingMsg()
    {
        while (!_stoppingToken.IsCancellationRequested)
        {
            var action = queue.Dequeue(_stoppingToken);
            _stoppingToken.ThrowIfCancellationRequested();
            var message = new Message(action.ClientId, action.Payload);
            if (senderService.SendAction(JsonSerializer.Serialize(message)))
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