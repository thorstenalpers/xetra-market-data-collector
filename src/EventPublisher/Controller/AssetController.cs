namespace EventPublisher.Controller;

using System.Net.Mime;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Events;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]

public class AssetController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public AssetController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    /// Initially creates a list of trading assets (stocks and etfs) by downloading an excel from Xetra.com and enriches meta information from Yahoo.com
    /// </summary>
    [HttpPost]
    [Route("Asset")]
    public async Task<IActionResult> CreateAssets()
    {
        await _publishEndpoint.Publish(new CreateAssetsRequested());
        return Ok($"Create assets requested");
    }


    /// <summary>
    /// Delete rarely traded assets and metric records
    /// </summary>
    /// <param name="daysWithNoCourses">Last days without courses</param>
    [HttpDelete]
    [Route("Asset")]
    public async Task<IActionResult> DeleteAssets([FromQuery] int daysWithNoCourses = 50)
    {
        await _publishEndpoint.Publish(new DeleteAssetsRequested
        {
            DaysWithNoCourses = daysWithNoCourses,
        });
        return Ok("Delete assets and metric records requested!");
    }

    /// <summary>
    /// Creates or updates the asset metric records by downloading historical data from Yahoo.com and then calculating metrics
    /// </summary>
    /// <param name="assetIds">all if empty</param>
    /// <param name="startDate">Start date for the historical data</param>
    /// <param name="endDate">Now, if empty</param>
    [HttpPost]
    [Route("Asset/RecordsByAssetIds")]
    public async Task<IActionResult> CreateRecordsByAssetIds(
    [FromQuery] List<int> assetIds = null,
    [FromQuery] string startDate = "2010-01-01",
    [FromQuery] string endDate = "")
    {
        if (string.IsNullOrWhiteSpace(endDate))
            endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

        await _publishEndpoint.Publish(new CreateAssetRecordsRequested
        {
            EndDate = DateTime.Parse(endDate).Date,
            StartDate = DateTime.Parse(startDate).Date,
            AssetIds = assetIds,
        });
        return Ok($"Update asset records requested");
    }

    /// <summary>
    /// Creates or updates the asset metric records by downloading historical data from Yahoo.com and then calculating metrics
    /// </summary>
    /// <param name="symbols">e.g. SAP.DE, all if empty</param>
    /// <param name="startDate">Start date for the historical data</param>
    /// <param name="endDate">Now, if empty</param>
    [HttpPost]
    [Route("Asset/RecordsBySymbols")]
    public async Task<IActionResult> CreateRecordsBySymbols(
    [FromQuery] List<string> symbols = null,
    [FromQuery] string startDate = "2010-01-01",
    [FromQuery] string endDate = "")
    {
        if (string.IsNullOrWhiteSpace(endDate))
            endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

        await _publishEndpoint.Publish(new CreateAssetRecordsRequested
        {
            EndDate = DateTime.Parse(endDate).Date,
            StartDate = DateTime.Parse(startDate).Date,
            Symbols = symbols,
        });
        return Ok($"Update asset records requested");
    }
}

