using Common;
using Microsoft.Extensions.Hosting;
using Producer.Contracts;

namespace WorkQueue;

public class Worker (IRabbitMqSender sender): BackgroundService
{
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(
            async (_) =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    Console.WriteLine("Enter message, please");
                    var message = Console.ReadLine();
                    if (message!.Empty())
                    {
                        continue;
                    }
                    var response = await sender.SendActionAsync(message!);
                    Console.WriteLine($"Response: {response}");
                }
            },
            stoppingToken,
            TaskCreationOptions.LongRunning
        );
    }
}