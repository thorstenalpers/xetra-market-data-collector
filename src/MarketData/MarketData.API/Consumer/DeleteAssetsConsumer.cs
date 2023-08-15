namespace MarketData.API.Consumer;

using MarketData.Application.Services.Interfaces;
using MassTransit;
using Newtonsoft.Json;
using Shared.Events;
using System.Threading.Tasks;

public class DeleteAssetsConsumer : IConsumer<DeleteAssetsRequested>
{
    private readonly IAssetsService _assetsService;
    private readonly ILogger<DeleteAssetsConsumer> _logger;
    public DeleteAssetsConsumer(ILogger<DeleteAssetsConsumer> logger, IAssetsService assetsService)
    {
        _logger = logger;
        _assetsService = assetsService;
    }

    public async Task Consume(ConsumeContext<DeleteAssetsRequested> context)
    {
        var msg = context.Message;
        _logger.LogInformation($"Starting {msg.GetType().Name} ... {JsonConvert.SerializeObject(msg)}");
        await _assetsService.DeleteAssets(msg.DaysWithNoCourses);
        _logger.LogInformation($"Finished {msg.GetType().Name} ... {JsonConvert.SerializeObject(msg)}");
    }
}
