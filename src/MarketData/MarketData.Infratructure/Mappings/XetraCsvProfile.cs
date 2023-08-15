namespace MarketData.Infratructure.Mappings;

using AutoMapper;
using MarketData.Domain.Entities;
using MarketData.Domain.ValueObjects;
using MarketData.Infratructure.Services.Models;

public class XetraCsvProfile : Profile
{
    public XetraCsvProfile()
    {
        CreateMap<XetraCsvEntry, Asset>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Instrument))
            .ForMember(dest => dest.Isin, opt => opt.MapFrom(src => src.ISIN))
            .ForMember(dest => dest.Mnemonic, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Mnemonic) ? null : src.Mnemonic))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Mnemonic) ? null : src.Mnemonic + ".DE"))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.InstrumentType == "CS" ? EAssetType.Stock : EAssetType.ETF));
    }
}