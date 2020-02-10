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
        entity.Property(e => e.FhirId).IsRequired(true).HasColumnName("fhir_id");
        //entity.Property(e => e.FhirVersion).IsRequired(true);
        entity.Property(e => e.IsCurrent).IsRequired(true).HasColumnName("is_current"); ;
        entity.Property(e => e.IsDeleted).IsRequired(true).HasColumnName("is_deleted"); ;
        entity.Property(e => e.Blob).IsRequired(false).HasColumnName("blob");
      });
    }

    public virtual DbSet<ResourceStore> ServerOptionsResource { get; set; }

  }
}
