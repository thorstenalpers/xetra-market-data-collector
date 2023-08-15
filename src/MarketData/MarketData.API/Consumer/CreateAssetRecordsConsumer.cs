namespace MarketData.API.Consumer;

using MarketData.Application.Services.Interfaces;
using MarketData.Domain.Entities;
using MarketData.Domain.Repositories;
using MarketData.Infratructure;
using MassTransit;
using Newtonsoft.Json;
using Shared.Events;
using System.Threading.Tasks;

public class CreateAssetRecordsConsumer : IConsumer<CreateAssetRecordsRequested>
{
    private readonly IAssetRecordService _assetRecordService;
    private readonly ILogger<CreateAssetRecordsConsumer> _logger;
    private readonly IRepository<Asset> _assetRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateAssetRecordsConsumer(ILogger<CreateAssetRecordsConsumer> logger,
        IPublishEndpoint publishEndpoint,
        IAssetRecordService assetRecordService,
        IRepository<Asset> assetRepository)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _assetRecordService = assetRecordService;
        _assetRepository = assetRepository;
    }

    public async Task Consume(ConsumeContext<CreateAssetRecordsRequested> context)
    {
        var msg = context.Message;
        _logger.LogInformation($"Starting {msg.GetType().Name} ... {JsonConvert.SerializeObject(msg)}");

        if (msg.AssetIds == null || !msg.AssetIds.Any())
        {
            var assets = await _assetRepository.ListAsync();
            var lsAssetIds = assets.Select(e => e.Id).ToList().SplitIntoBatches(100);
            await _publishEndpoint.PublishBatch(lsAssetIds.Select(newAssetIds => new CreateAssetRecordsRequested
            {
                StartDate = msg.StartDate,
                EndDate = msg.EndDate,
                AssetIds = newAssetIds
            }));
            _logger.LogInformation($"Created {lsAssetIds.Count} CreateAssetRecordsRequested events");
            return;
        }

        await _assetRecordService.CreateRecords(msg.AssetIds, msg.StartDate, msg.EndDate);
        _logger.LogInformation($"Finished {msg.GetType().Name} ... {JsonConvert.SerializeObject(msg)}");
    }
}
