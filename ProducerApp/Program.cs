
using Amazon.Runtime.CredentialManagement;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Contracts;
using Microsoft.Extensions.Logging;


new CredentialProfileStoreChain().TryGetAWSCredentials("default", out var awsCredentials);
var producer =  Host.CreateDefaultBuilder()
    .ConfigureLogging(n =>
    {
        n.AddConsole();
        n.SetMinimumLevel(LogLevel.Trace);
    })
    .ConfigureServices((hostContext, services) =>
    {
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
    .Build();

producer.Start();

var messageSender = producer.Services.GetRequiredService<MessageSender>();
while (true)
{
    await messageSender.SendMessage(Console.ReadLine()!);
}

class MessageSender 
{
    private readonly IBus _bus;

    public MessageSender(IBus bus)
    {
        _bus = bus;
    }

    public async Task SendMessage(string body)
    {
        var message = new Message 
        {
            Body = body
        };
        
        var client = _bus.CreateRequestClient<Message>(TimeSpan.FromSeconds(60));
        var responseMessage = await client.GetResponse<MessageResponse>(message);
        Console.WriteLine("Received " + responseMessage.Message.Body);
    }
}