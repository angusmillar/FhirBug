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
using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Bug.Data.Seeding;

namespace Bug.Data
{
  public class AppDbContext : DbContext, IUnitOfWork
  {
    private const bool GenerateNonStaticSeedData = false;
    public virtual DbSet<ResourceStore> ResourceStore { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.ResourceType> ResourceType { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.FhirVersion> FhirVersion { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.Method> Method { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.HttpStatusCode> HttpStatusCode { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.SearchParameterResourceType> SearchParameterResourceType { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.SearchParameterTargetResourceType> SearchParameterTargetResourceType { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.SearchParameterComponent> SearchParameterComponent { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.SearchParamType> SearchParamType { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.SearchParameter> SearchParameter { get; set; } = null!;


    public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options) { }

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
      DateTime DateTimeNow = DateTimeOffset.Now.ToZulu();
      base.OnModelCreating(builder);

      //##### ResourceStore #################################################
      builder.Entity<ResourceStore>(entity =>
      {
        SetupBaseIntKeyProperties(entity);
        entity.Property(e => e.ResourceId).HasColumnName("resource_id").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.FhirIdMaxLength); ;
        entity.Property(e => e.VersionId).HasColumnName("version_id").IsRequired(true);
        entity.Property(e => e.IsCurrent).HasColumnName("is_current").IsRequired(true); ;
        entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired(true); ;
        entity.Property(e => e.LastUpdated).HasColumnName("last_updated").IsRequired(true);
        entity.Property(e => e.ResourceBlob).HasColumnName("resource_blob").IsRequired(false);
        entity.Property(e => e.FkResourceTypeId).HasColumnName("fk_resourcetype_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.FkFhirVersionId).HasColumnName("fk_fhirversion_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.FkMethodId).HasColumnName("fk_method_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.FkHttpStatusCodeId).HasColumnName("fk_httpstatuscode_id").IsRequired(true);

        entity.HasOne(x => x.ResourceType)
        .WithMany()
        .HasForeignKey(x => x.FkResourceTypeId);

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
        entity.HasIndex(x => new { x.FkFhirVersionId, x.FkResourceTypeId, x.ResourceId, x.VersionId, })
          .HasName("UniqueIx_FhirVer_ResType_ResId_ResVer")
          .IsUnique();

        //We often order by LastUpdated
        entity.HasIndex(x => new { x.LastUpdated })
          .HasName("Ix_LastUpdated");

      });

      //##### ResourceName #################################################

      builder.Entity<Logic.DomainModel.ResourceType>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasConversion<int>();
        entity.Property(e => e.Created).HasColumnName("created").IsRequired(true);
        entity.Property(e => e.Updated).HasColumnName("updated").IsRequired(true);
        entity.Property(x => x.Code).IsRequired(true).HasColumnName("code").HasMaxLength(DatabaseMetaData.FieldLength.ResourceTypeStringMaxLength);
      });
      
      //builder.Entity<Logic.DomainModel.ResourceType>().HasData(Seeding.ResourceTypeSeed.GetSeedData(DateTimeNow));

      //##### FhirVersion #################################################

      builder.Entity<Logic.DomainModel.FhirVersion>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasConversion<int>();
        entity.Property(e => e.Created).HasColumnName("created").IsRequired(true);
        entity.Property(e => e.Updated).HasColumnName("updated").IsRequired(true);
        entity.Property(x => x.Code).IsRequired(true).HasColumnName("code").HasMaxLength(DatabaseMetaData.FieldLength.CodeMaxLength);
      });

      //Seed data
      //builder.Entity<Logic.DomainModel.FhirVersion>().HasData(
      //  new Logic.DomainModel.FhirVersion() { Id = Common.Enums.FhirVersion.Stu3, Code = Common.Enums.FhirVersion.Stu3.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Logic.DomainModel.FhirVersion() { Id = Common.Enums.FhirVersion.R4, Code = Common.Enums.FhirVersion.R4.GetCode(), Created = DateTimeNow, Updated = DateTimeNow });

      //##### Method #################################################

      builder.Entity<Method>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasConversion<int>(); ;
        entity.Property(e => e.Created).HasColumnName("created").IsRequired(true);
        entity.Property(e => e.Updated).HasColumnName("updated").IsRequired(true);
        entity.Property(x => x.Code).HasColumnName("code").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.CodeMaxLength); ;
      });

      ////Seed data
      //builder.Entity<Method>().HasData(
      //  new Method() { Id = HttpVerb.DELETE, Code = HttpVerb.DELETE.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Method() { Id = HttpVerb.GET, Code = HttpVerb.GET.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Method() { Id = HttpVerb.HEAD, Code = HttpVerb.HEAD.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Method() { Id = HttpVerb.PATCH, Code = HttpVerb.PATCH.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Method() { Id = HttpVerb.POST, Code = HttpVerb.POST.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Method() { Id = HttpVerb.PUT, Code = HttpVerb.PUT.GetCode(), Created = DateTimeNow, Updated = DateTimeNow });

      //##### HttpStatusCode #################################################

      builder.Entity<HttpStatusCode>(entity =>
      {
        SetupBaseIntKeyProperties(entity);
        entity.Property(x => x.Code).HasColumnName("code").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.CodeMaxLength); 
        entity.Property(x => x.Number).HasColumnName("number").IsRequired(true).HasConversion<int>();

        entity.HasIndex(x => new { x.Number, })
        .HasName("Unique_Number")
        .IsUnique();
      });

      //Seed data
      //builder.Entity<HttpStatusCode>().HasData(GetHttpStatusSeedData(DateTimeNow));

      //##### SearchParameterResourceType #################################################

      builder.Entity<SearchParameterResourceType>(entity =>
      {
        SetupBaseIntKeyProperties(entity);
        entity.Property(x => x.FkResourceTypeId).HasColumnName("fk_resourcetype_id").IsRequired(true).HasConversion<int>();
        
        entity.HasOne(x => x.SearchParameter)
        .WithMany(y => y.ResourceTypeList)
        .HasForeignKey(x => x.FkSearchParameterId);

        entity.HasOne(x => x.ResourceType)
        .WithMany()
        .HasForeignKey(x => x.FkResourceTypeId);
      });

      if (GenerateNonStaticSeedData)
      {
        //builder.Entity<SearchParameterResourceType>().HasData(Seeding.SearchParameterResourceTypeSeed.GetSeedData(DateTimeNow));
      }
        

      //##### SearchParameterTargetResourceType #################################################

      builder.Entity<SearchParameterTargetResourceType>(entity =>
      {
        SetupBaseIntKeyProperties(entity);
        entity.Property(x => x.FkResourceTypeId).HasColumnName("fk_resourcetype_id").IsRequired(true).HasConversion<int>();

        entity.HasOne(x => x.SearchParameter)
        .WithMany(y => y.TargetResourceTypeList)
        .HasForeignKey(x => x.FkSearchParameterId);

        entity.HasOne(x => x.ResourceType)
        .WithMany()
        .HasForeignKey(x => x.FkResourceTypeId);
      });

      if (GenerateNonStaticSeedData)
      {
        //builder.Entity<SearchParameterTargetResourceType>().HasData(Seeding.SearchParameterTargetResourceTypeSeed.GetSeedData(DateTimeNow));
      }

      //##### SearchParameterComponent #################################################

      builder.Entity<SearchParameterComponent>(entity =>
      {
        SetupBaseIntKeyProperties(entity);

        entity.Property(e => e.Definition).HasColumnName("definition").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);
        entity.Property(e => e.Expression).HasColumnName("expression").IsRequired(true);

        entity.HasOne(x => x.SearchParameter)
        .WithMany(y => y.ComponentList)
        .HasForeignKey(x => x.FkSearchParameterId);
      });

      if (GenerateNonStaticSeedData)
      {
        //builder.Entity<SearchParameterComponent>().HasData(Seeding.SearchParameterComponentSeed.GetSeedData(DateTimeNow));
      }
      //##### SearchParamType #################################################

      builder.Entity<Bug.Logic.DomainModel.SearchParamType>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id").HasConversion<int>(); ;
        entity.Property(e => e.Created).HasColumnName("created").IsRequired(true);
        entity.Property(e => e.Updated).HasColumnName("updated").IsRequired(true);
        entity.Property(x => x.Code).HasColumnName("code").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.CodeMaxLength);
      });

      //builder.Entity<Bug.Logic.DomainModel.SearchParamType>().HasData(
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Composite, Code = Bug.Common.Enums.SearchParamType.Composite.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Date, Code = Bug.Common.Enums.SearchParamType.Date.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Number, Code = Bug.Common.Enums.SearchParamType.Number.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Quantity, Code = Bug.Common.Enums.SearchParamType.Quantity.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Reference, Code = Bug.Common.Enums.SearchParamType.Reference.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Special, Code = Bug.Common.Enums.SearchParamType.Special.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.String, Code = Bug.Common.Enums.SearchParamType.String.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Token, Code = Bug.Common.Enums.SearchParamType.Token.GetCode(), Created = DateTimeNow, Updated = DateTimeNow },
      //  new Bug.Logic.DomainModel.SearchParamType() { Id = Bug.Common.Enums.SearchParamType.Uri, Code = Bug.Common.Enums.SearchParamType.Uri.GetCode(), Created = DateTimeNow, Updated = DateTimeNow });


      //##### SearchParameter #################################################

      builder.Entity<SearchParameter>(entity =>
      {
        SetupBaseIntKeyProperties(entity);

        entity.Property(x => x.Name).HasColumnName("name").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.NameMaxLength);
        entity.Property(x => x.Description).HasColumnName("description").IsRequired(false);
        entity.Property(x => x.FkSearchParamTypeId).HasColumnName("fk_searchparamtype_id").IsRequired(true).HasConversion<int>();
        entity.Property(x => x.Url).HasColumnName("url").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);
        entity.Property(x => x.FhirPath).HasColumnName("fhir_path").IsRequired(false);
        entity.Property(x => x.FkFhirVersionId).HasColumnName("fk_fhirversion_id").IsRequired(true).HasConversion<int>();

        entity.HasMany(x => x.ResourceTypeList)
        .WithOne(y => y.SearchParameter)
        .HasForeignKey(x => x.FkSearchParameterId);

        entity.HasMany(x => x.TargetResourceTypeList)
        .WithOne(y => y.SearchParameter)
        .HasForeignKey(x => x.FkSearchParameterId);

        entity.HasOne(x => x.SearchParamType)
        .WithMany()
        .HasForeignKey(x => x.FkSearchParamTypeId);

        entity.HasMany(x => x.ComponentList)
        .WithOne(y => y.SearchParameter)
        .HasForeignKey(x => x.FkSearchParameterId);

        entity.HasOne(x => x.FhirVersion)
        .WithMany()
        .HasForeignKey(x => x.FkFhirVersionId);
      });

      if (GenerateNonStaticSeedData)
      {
        //builder.Entity<SearchParameter>().HasData(Bug.Data.Seeding.SearchParameterSeed.GetSeedData(DateTimeNow));
      }
    }

    private static void SetupBaseIntKeyProperties<T>(EntityTypeBuilder<T> entity) where T :BaseIntKey
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).HasColumnName("id");
      entity.Property(e => e.Created).HasColumnName("created");
      entity.Property(e => e.Updated).HasColumnName("updated");
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
