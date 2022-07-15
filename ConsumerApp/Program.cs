
using Amazon.Runtime.CredentialManagement;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Contracts;
using Microsoft.Extensions.Logging;

new CredentialProfileStoreChain().TryGetAWSCredentials("default", out var awsCredentials);
await Host.CreateDefaultBuilder()
    .ConfigureLogging(n =>
    {
        n.AddConsole();
        n.SetMinimumLevel(LogLevel.Trace);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<MessageConsumer>();
            // x.UsingAmazonSqs((context, cfg) =>
            // {
            //     cfg.Host("eu-central-1", h =>
            //     {
            //         h.Credentials(awsCredentials);
            //     });
            //     cfg.ConfigureEndpoints(context);
            // });;
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
    .Build()
    .RunAsync();

public class MessageConsumer : IConsumer<Message>
{
    public async Task Consume(ConsumeContext<Message> context)
    {
        Console.WriteLine(context.Message.Body);
        await context.RespondAsync<MessageResponse>(new () 
        {
             Body = "Re " + context.Message.Body
        }); 
    }
}
