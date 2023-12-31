﻿namespace MarketData.Infrastructure.Services;

using CsvHelper;
using CsvHelper.Configuration;
using MarketData.Application.Exceptions;
using MarketData.Application.Interfaces;
using MarketData.Application.Options;
using MarketData.Application.Repositories;
using MarketData.Application.ValueObjects;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class XetraWebScraper : IXetraWebScraper, IScopedService
{
    private readonly HttpClient _httpClient;
    private readonly XetraOptions _xetraOptions;

    public XetraWebScraper(IHttpClientFactory httpClientFactory, IOptions<XetraOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(XetraWebScraper));
        _xetraOptions = options.Value;
    }

    public async Task<List<XetraCsvEntry>> GetTradableInstruments()
    {
        try
        {
            var response = await _httpClient.GetAsync(_xetraOptions.DownloadUrl);
            response.EnsureSuccessStatusCode();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ";",
            };

            using var reader = new StreamReader(response.Content.ReadAsStream());
            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.Read();
            csv.Context.RegisterClassMap<XetraCsvClassMap>();
            var records = csv.GetRecords<XetraCsvEntry>()?.ToList();
            return records;
        }
        catch (Exception ex)
        {
            throw new MarketDataException($"Failed to download from {_xetraOptions.DownloadUrl}", ex); ;
        }
    }
}
