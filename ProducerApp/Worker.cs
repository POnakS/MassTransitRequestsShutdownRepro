using Microsoft.Extensions.Hosting;

namespace ProducerApp;

public class Worker : BackgroundService
{
    private readonly MessageSender _sender;

    public Worker(MessageSender sender)
    {
        _sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var input = await Console.In.ReadLineAsync()!;

            if (input == null)
            {
                return;
            }

            await _sender.SendMessage(input);
        }
    }
}