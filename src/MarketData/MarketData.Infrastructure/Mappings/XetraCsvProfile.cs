namespace MarketData.Infrastructure.Mappings;

using AutoMapper;
using MarketData.Application.Entities;
using MarketData.Application.ValueObjects;

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