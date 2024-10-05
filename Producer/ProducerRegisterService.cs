using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Producer.Contracts;
using Producer.Options;

namespace Producer;

public static class ProducerRegisterService
{
    public static void AddProducerServices(this IServiceCollection services)
    {
        services.AddSingleton<ActionsQueue>();
        services.AddSingleton<IActionStorage, ActionsStorage>();
        
        services.AddScoped<ISenderService, Sender>();
        services.AddScoped<IReceiveHandler, ReceiveHandler>();
        services.AddScoped<IRabbitMqSender,RabbitMqSender>();
        
        services.AddHostedService<BackgroundProcess>();
    }

    public static void AddProducerConfiguration(this IConfigurationManager configuration)
    {
        configuration.GetSection(nameof(RabbitMqOptions)).Bind(new RabbitMqOptions());
    }
}