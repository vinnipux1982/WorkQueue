using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;
using WorkQueue;

Console.WriteLine($"Work queue, version {VersionInfo.Version}");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddProducerServices();
builder.Services.AddHostedService<Worker>();
builder.Configuration.AddProducerConfiguration();

var host = builder.Build();

var cancellationSource = new CancellationTokenSource();

await host.RunAsync(cancellationSource.Token);

cancellationSource.Cancel();

Console.WriteLine("Finished");