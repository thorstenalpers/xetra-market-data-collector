namespace MarketData.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MarketData.Domain.Entities;
using MarketData.Domain.ValueObjects;

public class AssetMetricConfiguration : IEntityTypeConfiguration<AssetMetric>
{
    public void Configure(EntityTypeBuilder<AssetMetric> builder)
    {
        builder.ToTable("AssetMetrics", "TradeX");
        builder.HasKey(ci => ci.Id);
        builder.Property(d => d.Type).HasConversion<string>();
        var initialData = Enum.GetValues<EAssetMetricType>()
            .Select((e, idx) => new AssetMetric
            {
                Id = (int)e,
                Type = e.ToString(),
                CreatedAt = new DateTime(2023, 08, 1).Date
            });
        builder.HasData(initialData);
    }
}
