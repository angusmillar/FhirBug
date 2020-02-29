using Microsoft.EntityFrameworkCore;
using System;
using Bug.Logic.DomainModel;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.Repository;

namespace Bug.Data
{
  public class AppDbContext : DbContext, IUnitOfWork
  {
    public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ResourceStore>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.ResourceId).IsRequired(true).HasColumnName("resource_id");
        entity.Property(e => e.VersionId).IsRequired(true).HasColumnName("version_id"); ;
        entity.Property(e => e.IsCurrent).IsRequired(true).HasColumnName("is_current");
        entity.Property(e => e.IsDeleted).IsRequired(true).HasColumnName("is_deleted");
        entity.Property(e => e.LastUpdated).IsRequired(true).HasColumnName("last_updated");
        entity.Property(e => e.ResourceBlob).IsRequired(false).HasColumnName("resource_blob");
        entity.Property(e => e.FkResourceNameId).IsRequired(true).HasColumnName("fk_resource_name_id");
        entity.Property(e => e.FkFhirVersionId).IsRequired(true).HasColumnName("fk_fhir_version_id");
        entity.Property(e => e.FkMethodId).IsRequired(true).HasColumnName("fk_method_id");

        entity.HasOne(x => x.ResourceName)
        .WithMany()
        .HasForeignKey(x => x.FkResourceNameId);

        entity.HasOne(x => x.FhirVersion)
        .WithMany()
        .HasForeignKey(x => x.FkFhirVersionId);

        entity.HasOne(x => x.Method)
        .WithMany()
        .HasForeignKey(x => x.FkMethodId);

        //Ensure that no two resources have the same ResourceId, VersionId, for the same FHIR Version and Resource Name
        entity.HasIndex(x => new { x.FkFhirVersionId, x.FkResourceNameId, x.ResourceId, x.VersionId, } )
        .HasName("Unique_FhirVer_ResName_ResId_ResVer")
        .IsUnique();
      });

      modelBuilder.Entity<ResourceName>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(x => x.Name).IsRequired(true).HasColumnName("name");

      });

      modelBuilder.Entity<FhirVersion>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(x => x.FhirMajorVersion).IsRequired(true).HasColumnName("fhir_major_version");
        entity.Property(x => x.VersionCode).IsRequired(true).HasColumnName("code");
      });

      modelBuilder.Entity<Method>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(x => x.Code).IsRequired(true).HasColumnName("code");
        entity.Property(x => x.HttpVerb).IsRequired(true).HasColumnName("http_verb");
      });
    }

    public async Task<IBugDbContextTransaction> BeginTransactionAsync()
    {
      IDbContextTransaction DbContextTransaction = await this.Database.BeginTransactionAsync();
      return new BugDbContextTransaction(DbContextTransaction);
    }
    public virtual DbSet<ResourceStore> ResourceStore { get; set; } = null!;
    public virtual DbSet<ResourceName> ResourceName { get; set; } = null!;
    public virtual DbSet<FhirVersion> FhirVersion { get; set; } = null!;

  }
}
