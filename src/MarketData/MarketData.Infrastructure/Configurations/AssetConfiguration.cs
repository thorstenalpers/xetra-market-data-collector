namespace MarketData.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MarketData.Application.Entities;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets", "TradeX");
        builder.HasKey(ci => ci.Id);
        builder.HasIndex(p => p.Isin).IsUnique();
        builder.Property(d => d.Type).HasConversion<string>();
    }
}
