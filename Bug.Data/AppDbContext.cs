using Microsoft.EntityFrameworkCore;
using System;
using Bug.Logic.DomainModel;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using Bug.Logic.Interfaces.Repository;
using Bug.Common.Enums;
using System.Collections.Generic;
using System.Linq;
using Bug.Common.DateTimeTools;

namespace Bug.Data
{
  public class AppDbContext : DbContext, IUnitOfWork
  {
    public virtual DbSet<ResourceStore> ResourceStore { get; set; } = null!;
    public virtual DbSet<ResourceName> ResourceName { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.FhirVersion> FhirVersion { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options) { }

    
    protected override void OnModelCreating(ModelBuilder builder)
    {
      DateTime DateTimeNow = DateTimeOffset.Now.ToZulu();
      base.OnModelCreating(builder);

      builder.Entity<ResourceStore>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Created).HasColumnName("created");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(e => e.ResourceId).HasColumnName("resource_id").IsRequired(true);
        entity.Property(e => e.VersionId).HasColumnName("version_id").IsRequired(true); 
        entity.Property(e => e.IsCurrent).HasColumnName("is_current").IsRequired(true); ;
        entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired(true); ;
        entity.Property(e => e.LastUpdated).HasColumnName("last_updated").IsRequired(true);
        entity.Property(e => e.ResourceBlob).HasColumnName("resource_blob").IsRequired(false);
        entity.Property(e => e.FkResourceNameId).HasColumnName("fk_resourcename_id").IsRequired(true);
        entity.Property(e => e.FkFhirVersionId).HasColumnName("fk_fhirversion_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.FkMethodId).HasColumnName("fk_method_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.FkHttpStatusCodeId).HasColumnName("fk_httpstatuscode_id").IsRequired(true);

        entity.HasOne(x => x.ResourceName)
        .WithMany()
        .HasForeignKey(x => x.FkResourceNameId);

        entity.HasOne(x => x.FhirVersion)
        .WithMany()
        .HasForeignKey(x => x.FkFhirVersionId);

        entity.HasOne(x => x.Method)
        .WithMany()
        .HasForeignKey(x => x.FkMethodId);

        entity.HasOne(x => x.HttpStatusCode)
        .WithMany()
        .HasForeignKey(x => x.FkHttpStatusCodeId);

        //Ensure that no two resources have the same ResourceId, VersionId, for the same FHIR Version and Resource Name
        entity.HasIndex(x => new { x.FkFhirVersionId, x.FkResourceNameId, x.ResourceId, x.VersionId, } )
        .HasName("Unique_FhirVer_ResName_ResId_ResVer")
        .IsUnique();
      });

      builder.Entity<ResourceName>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Created).HasColumnName("created");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(x => x.Name).IsRequired(true).HasColumnName("name");

      });

      builder.Entity<Logic.DomainModel.FhirVersion>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasConversion<int>();
        entity.Property(e => e.Created).HasColumnName("created").IsRequired(true);
        entity.Property(e => e.Updated).HasColumnName("updated").IsRequired(true);        
        entity.Property(x => x.Code).IsRequired(true).HasColumnName("code");
      });

      //Seed data
      builder.Entity<Logic.DomainModel.FhirVersion>().HasData(
        new Logic.DomainModel.FhirVersion() { Id = Common.Enums.FhirVersion.Stu3, Code = Common.Enums.FhirVersion.Stu3.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
        new Logic.DomainModel.FhirVersion() { Id = Common.Enums.FhirVersion.R4, Code = Common.Enums.FhirVersion.R4.GetCode(), Created = DateTimeNow, Updated = DateTimeNow });
        

      builder.Entity<Method>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasConversion<int>(); ;
        entity.Property(e => e.Created).HasColumnName("created").IsRequired(true);
        entity.Property(e => e.Updated).HasColumnName("updated").IsRequired(true);
        entity.Property(x => x.Code).HasColumnName("code").IsRequired(true);        
      });

      //Seed data
      builder.Entity<Method>().HasData(
        new Method() { Id = HttpVerb.DELETE, Code = HttpVerb.DELETE.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
        new Method() { Id = HttpVerb.GET, Code = HttpVerb.GET.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
        new Method() { Id = HttpVerb.HEAD, Code = HttpVerb.HEAD.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
        new Method() { Id = HttpVerb.PATCH, Code = HttpVerb.PATCH.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
        new Method() { Id = HttpVerb.POST, Code = HttpVerb.POST.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
        new Method() { Id = HttpVerb.PUT, Code = HttpVerb.PUT.GetCode(), Created = DateTimeNow, Updated = DateTimeNow });

      builder.Entity<HttpStatusCode>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasConversion<int>();
        entity.Property(e => e.Created).HasColumnName("created");
        entity.Property(e => e.Updated).HasColumnName("updated");
        entity.Property(x => x.Code).HasColumnName("code").IsRequired(true);
        entity.Property(x => x.Number).HasColumnName("number").IsRequired(true).HasConversion<int>();

        entity.HasIndex(x => new { x.Number, })
        .HasName("Unique_Number")
        .IsUnique();

      });

      //Seed data
      builder.Entity<HttpStatusCode>().HasData(GetHttpStatusSeedData(DateTimeNow));
  
    }

    public async Task<IBugDbContextTransaction> BeginTransactionAsync()
    {      
      if (this.Database.CurrentTransaction is object)
      {
        return new BugDbContextTransaction(this.Database.CurrentTransaction);
      }
      else
      {
        return new BugDbContextTransaction(await this.Database.BeginTransactionAsync());
      }            
    }

    private HttpStatusCode[] GetHttpStatusSeedData(DateTime dateTimeNow)
    {
      int Key = 1;      
      var ResultList = new List<HttpStatusCode>();
      var HttpStatusCodeList = Enum.GetValues(typeof(System.Net.HttpStatusCode)).Cast<System.Net.HttpStatusCode>();
      // HttpStatusCode enum uses multiple integer values for the same value, e.g.
      // System.Net.HttpStatusCode uses these:
      // Ambiguous = 300,
      // MultipleChoices = 300,
      // Moved = 301,      
      // MovedPermanently = 301,
      // Redirect = 302
      // Found = 302
      // RedirectMethod = 303
      // SeeOther = 303
      // RedirectKeepVerb = 307
      // TemporaryRedirect = 307
      //So here we filter them out with HttpStatusCodeList.Distinct()
      foreach (System.Net.HttpStatusCode HttpStatusEnum in HttpStatusCodeList.Distinct())
      {
        ResultList.Add(new HttpStatusCode()
        {
          Id = Key,          
          Code = HttpStatusEnum.ToString(),
          Number = HttpStatusEnum,
          Created = dateTimeNow,
          Updated = dateTimeNow
        });
        Key++;
      }
      return ResultList.ToArray();
    }

  }
}
