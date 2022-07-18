using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

public class MessageSender 
{
    private readonly IBus _bus;
    private readonly ILogger<MessageSender> _logger;

    public MessageSender(IBus bus, ILogger<MessageSender> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task SendMessage(string body)
    {
        var message = new Message 
        {
            Body = body
        };
        
        var client = _bus.CreateRequestClient<Message>(TimeSpan.FromSeconds(30));

        try
        {
            _logger.LogInformation("Sending " + message.Body);
            var responseMessage = await client.GetResponse<MessageResponse>(message);
            _logger.LogInformation("Received " + responseMessage.Message.Body);
        }
        catch
        {
            _logger.LogInformation("Receive failed with exception");
        }
    }
}