namespace MarketData.Infrastructure.Mappings;

using AutoMapper;
using MarketData.Application.Entities;
using MarketData.Application.ValueObjects;

public class AssetMetaDataProfile : Profile
{
    public AssetMetaDataProfile()
    {
        CreateMap<AssetMetaData, Asset>();
    }
}