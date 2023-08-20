namespace MarketData.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MarketData.Application.Entities;

public class AssetRecordConfiguration : IEntityTypeConfiguration<AssetRecord>
{
    public void Configure(EntityTypeBuilder<AssetRecord> builder)
    {
        builder.ToTable("AssetRecords", "TradeX");
        builder.HasKey(ci => ci.Id);
        builder.HasIndex(p => new { p.Date, p.AssetId, p.AssetMetricId }).IsUnique();
    }
}
