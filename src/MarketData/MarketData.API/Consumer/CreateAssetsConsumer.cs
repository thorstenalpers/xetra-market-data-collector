namespace MarketData.API.Consumer;

using MarketData.Application.Interfaces;
using MassTransit;
using Newtonsoft.Json;
using Shared.Events;
using System.Threading.Tasks;

public class CreateAssetsConsumer : IConsumer<CreateAssetsRequested>
{
    private readonly IAssetsService _assetsService;
    private readonly ILogger<CreateAssetsConsumer> _logger;
    public CreateAssetsConsumer(ILogger<CreateAssetsConsumer> logger, IAssetsService assetsService)
    {
        _logger = logger;
        _assetsService = assetsService;
    }
    public async Task Consume(ConsumeContext<CreateAssetsRequested> context)
    {
        var msg = context.Message;
        await _assetsService.CreateAssets();
        _logger.LogInformation($"Finished {msg.GetType().Name} ... {JsonConvert.SerializeObject(msg)}");
    }
}
