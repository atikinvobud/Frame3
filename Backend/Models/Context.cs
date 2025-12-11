using Backend.Models.Configs;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models;
public class Context : DbContext
{
    public DbSet<IssFetchLog> IssFetchLogs { get; set; }
    public DbSet<OsdrItem> OsdrItems { get; set; }
    public DbSet<SpaceCache> SpaceCache { get; set; }
    public DbSet<TelemetryEntity> Telemetry { get; set; }

    public Context(DbContextOptions<Context> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new IssFetchLogConfig());
        modelBuilder.ApplyConfiguration(new OsdrItemConfig());
        modelBuilder.ApplyConfiguration(new SpaceCacheConfig());
        modelBuilder.ApplyConfiguration(new TelemetryConfig());
    }

}