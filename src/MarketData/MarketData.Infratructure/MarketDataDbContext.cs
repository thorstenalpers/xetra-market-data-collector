namespace MarketData.Infratructure;

using Microsoft.EntityFrameworkCore;
public class MarketDataDbContext : DbContext
{
    private static readonly object migrationLock = new object();

    public MarketDataDbContext(DbContextOptions<MarketDataDbContext> options) : base(options)
    {
        lock (migrationLock)
        {
            Database?.Migrate();
            Database?.SetCommandTimeout(100000);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketDataDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
