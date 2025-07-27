using Microsoft.EntityFrameworkCore;
using TrainComponent.Domain.Entities;

namespace TrainComponent.Infrastructure.Persistence;

public class TrainDbContext(DbContextOptions<TrainDbContext> options) : DbContext(options)
{
    public DbSet<Component> Components { get; set; }
    public DbSet<ComponentQuantity> ComponentQuantities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

            entity.Property(e => e.UniqueNumber).HasMaxLength(50).IsRequired();

            entity.HasIndex(e => e.UniqueNumber).IsUnique();

            entity.HasIndex(e => e.Name);

            entity
                .HasOne(e => e.Quantity)
                .WithOne(q => q.Component)
                .HasForeignKey<ComponentQuantity>(q => q.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ComponentQuantity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.ComponentId).IsUnique(); // 1-to-1

            entity.Property(e => e.Quantity).IsRequired();

            entity.ToTable(entity =>
            {
                entity.HasCheckConstraint("CK_Quantity_Positive", "[Quantity] > 0");
            });
        });
    }
}
