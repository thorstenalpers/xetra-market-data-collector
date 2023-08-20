namespace MarketData.Application.Entities;

using Ardalis.Specification;
using System.ComponentModel.DataAnnotations.Schema;


public abstract class BaseEntity : IEntity<int>
{
    public virtual int Id { get; set; }
    [Column(TypeName = "date")] public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
