using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;
using Serilog;
using WorkQueue;

Console.WriteLine($"Work queue, version {VersionInfo.Version}");

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");
    
builder.Services.AddSerilog();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
logger.Information("Start WorkQueue");
builder.Services.AddProducerServices();
builder.Services.AddHostedService<Worker>();

builder.Configuration.AddProducerConfiguration();

var host = builder.Build();

var cancellationSource = new CancellationTokenSource();

await host.RunAsync(cancellationSource.Token);

cancellationSource.Cancel();

Console.WriteLine("Finished");