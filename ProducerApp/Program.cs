
using Amazon.Runtime.CredentialManagement;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProducerApp;

new CredentialProfileStoreChain().TryGetAWSCredentials("default", out var awsCredentials);
await  Host.CreateDefaultBuilder()
    .ConfigureLogging(n =>
    {
        n.AddConsole(c =>
        {
            c.TimestampFormat = "[HH:mm:ss] ";
        });
        n.SetMinimumLevel(LogLevel.Trace);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<HostOptions>(o =>
        {
            o.ShutdownTimeout = TimeSpan.FromSeconds(60);
        });
        services.AddHostedService<Worker>();
        services.AddTransient<MessageSender>();
        services.AddMassTransit(x =>
        {
            // x.UsingAmazonSqs((context, cfg) =>
            // {
            //     cfg.Host("eu-central-1", h =>
            //     {
            //         h.Credentials(awsCredentials);
            //     });
            //     cfg.ConfigureEndpoints(context);
            // });
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .RunConsoleAsync();