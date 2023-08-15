namespace MarketData.Infratructure.Mappings;

using AutoMapper;
using MarketData.Domain.Entities;
using MarketData.Domain.ValueObjects;

public class AssetMetaDataProfile : Profile
{
    public AssetMetaDataProfile()
    {
        CreateMap<AssetMetaData, Asset>();
    }
}