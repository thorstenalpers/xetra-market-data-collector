namespace MarketData.Application.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class AssetRecord : BaseEntity
{
    [Column(TypeName = "date")] public DateTime Date { get; set; }
    public double Value { get; set; }

    public int AssetId { get; set; }
    public virtual Asset Asset { get; set; }

    public int AssetMetricId { get; set; }
    public virtual AssetMetric AssetMetric { get; set; }
}
