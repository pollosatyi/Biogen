using Biogen.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biogen.DAl.Repository;

public class Context : DbContext
{
    public DbSet<ImageDetectionOutcome>  ImageDetectionOutcomes { get; set; }
    
    
    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ImageDetectionOutcome>()
            .HasMany(o => o.DetectedItems)
            .WithOne()
            .HasForeignKey(d => d.ImageDetectionOutcomeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DetectedItem>()
            .Property(d => d.materials)
            .HasColumnType("jsonb");
    }
}

