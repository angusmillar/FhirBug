using Microsoft.EntityFrameworkCore;
using System;
using Bug.Logic.DomainModel;

namespace Bug.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ResourceStore>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.ResourceId).IsRequired(true).HasColumnName("resource_id");
        entity.Property(e => e.VersionId).IsRequired(true).HasColumnName("version_id"); ;
        entity.Property(e => e.IsCurrent).IsRequired(true).HasColumnName("is_current");
        entity.Property(e => e.IsDeleted).IsRequired(true).HasColumnName("is_deleted");
        entity.Property(e => e.LastUpdated).IsRequired(true).HasColumnName("last_updated");
        entity.Property(e => e.ResourceBlob).IsRequired(false).HasColumnName("resource_blob");
      });
    }

    public virtual DbSet<ResourceStore>? ResourceStore { get; set; }

  }
}
