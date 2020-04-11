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
using Bug.Common.DatabaseTools;

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
    public virtual DbSet<Logic.DomainModel.ServiceBaseUrl> ServiceBaseUrl { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.IndexDateTime> IndexDateTime { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.IndexQuantity> IndexQuantity { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.IndexReference> IndexReference { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.IndexString> IndexString { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.IndexToken> IndexToken { get; set; } = null!;
    public virtual DbSet<Logic.DomainModel.IndexUri> IndexUri { get; set; } = null!;

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
        entity.Property(e => e.ResourceId).HasColumnName("resource_id").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.FhirIdMaxLength);
        entity.Property(e => e.ContainedId).HasColumnName("contained_id").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.FhirIdMaxLength);
        entity.Property(e => e.VersionId).HasColumnName("version_id").IsRequired(true);
        entity.Property(e => e.IsCurrent).HasColumnName("is_current").IsRequired(true); ;
        entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired(true); ;
        entity.Property(e => e.LastUpdated).HasColumnName("last_updated").IsRequired(true);
        entity.Property(e => e.ResourceBlob).HasColumnName("resource_blob").IsRequired(false);
        entity.Property(e => e.ResourceTypeId).HasColumnName("resourcetype_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.FhirVersionId).HasColumnName("fhirversion_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.MethodId).HasColumnName("method_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.HttpStatusCodeId).HasColumnName("httpstatuscode_id").IsRequired(true);

        entity.HasOne(x => x.ResourceType).WithMany().HasForeignKey(x => x.ResourceTypeId);
        entity.HasOne(x => x.FhirVersion).WithMany().HasForeignKey(x => x.FhirVersionId);
        entity.HasOne(x => x.Method).WithMany().HasForeignKey(x => x.MethodId);
        entity.HasOne(x => x.HttpStatusCode).WithMany().HasForeignKey(x => x.HttpStatusCodeId);

        entity.HasMany(x => x.DateTimeIndexList).WithOne(v => v.ResourceStore).HasForeignKey(x => x.ResourceStoreId);
        entity.HasMany(x => x.QuantityIndexList).WithOne(v => v.ResourceStore).HasForeignKey(x => x.ResourceStoreId);
        entity.HasMany(x => x.ReferenceIndexList).WithOne(v => v.ResourceStore).HasForeignKey(x => x.ResourceStoreId);
        entity.HasMany(x => x.StringIndexList).WithOne(v => v.ResourceStore).HasForeignKey(x => x.ResourceStoreId);
        entity.HasMany(x => x.TokenIndexList).WithOne(v => v.ResourceStore).HasForeignKey(x => x.ResourceStoreId);
        entity.HasMany(x => x.UriIndexList).WithOne(v => v.ResourceStore).HasForeignKey(x => x.ResourceStoreId);

        //Ensure that no two resources have the same ResourceId, VersionId, for the same FHIR Version and Resource Name
        entity.HasIndex(x => new { x.FhirVersionId, x.ResourceTypeId, x.ResourceId, x.ContainedId, x.VersionId, })
          .HasName("UniqueIx_ResourceStore_FhirVer_ResType_ResId_ContId_ResVer")
          .IsUnique();

        //We often order by LastUpdated
        entity.HasIndex(x => x.LastUpdated)
          .HasName("Ix_ResourceStore_LastUpdated");

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

        entity.HasIndex(x => x.Number)
        .HasName("UniqueIx_HttpStatusCode_number")
        .IsUnique();
      });

      //Seed data
      //builder.Entity<HttpStatusCode>().HasData(GetHttpStatusSeedData(DateTimeNow));

      //##### SearchParameterResourceType #################################################

      builder.Entity<SearchParameterResourceType>(entity =>
      {
        SetupBaseIntKeyProperties(entity);
        entity.Property(x => x.SearchParameterId).HasColumnName("searchparameter_id");
        entity.Property(x => x.ResourceTypeId).HasColumnName("resourcetype_id").IsRequired(true).HasConversion<int>();

        entity.HasOne(x => x.SearchParameter)
        .WithMany(y => y.ResourceTypeList)
        .HasForeignKey(x => x.SearchParameterId);

        entity.HasOne(x => x.ResourceType)
        .WithMany()
        .HasForeignKey(x => x.ResourceTypeId);
      });

      if (GenerateNonStaticSeedData)
      {
        //builder.Entity<SearchParameterResourceType>().HasData(Seeding.SearchParameterResourceTypeSeed.GetSeedData(DateTimeNow));
      }


      //##### SearchParameterTargetResourceType #################################################

      builder.Entity<SearchParameterTargetResourceType>(entity =>
      {
        SetupBaseIntKeyProperties(entity);
        entity.Property(x => x.ResourceTypeId).HasColumnName("resourcetype_id").IsRequired(true).HasConversion<int>();
        entity.Property(x => x.SearchParameterId).HasColumnName("searchparameter_id");

        entity.HasOne(x => x.SearchParameter)
        .WithMany(y => y.TargetResourceTypeList)
        .HasForeignKey(x => x.SearchParameterId);

        entity.HasOne(x => x.ResourceType)
        .WithMany()
        .HasForeignKey(x => x.ResourceTypeId);
      });

      if (GenerateNonStaticSeedData)
      {
        //builder.Entity<SearchParameterTargetResourceType>().HasData(Seeding.SearchParameterTargetResourceTypeSeed.GetSeedData(DateTimeNow));
      }

      //##### SearchParameterComponent #################################################

      builder.Entity<SearchParameterComponent>(entity =>
      {
        SetupBaseIntKeyProperties(entity);

        entity.Property(x => x.SearchParameterId).HasColumnName("searchparameter_id");
        entity.Property(e => e.Definition).HasColumnName("definition").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);
        entity.Property(e => e.Expression).HasColumnName("expression").IsRequired(true);

        entity.HasOne(x => x.SearchParameter)
        .WithMany(y => y.ComponentList)
        .HasForeignKey(x => x.SearchParameterId);       
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
        entity.Property(x => x.SearchParamTypeId).HasColumnName("searchparamtype_id").IsRequired(true).HasConversion<int>();
        entity.Property(x => x.Url).HasColumnName("url").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);
        entity.Property(x => x.FhirPath).HasColumnName("fhir_path").IsRequired(false);
        entity.Property(x => x.FhirVersionId).HasColumnName("fhirversion_id").IsRequired(true).HasConversion<int>();

        entity.HasMany(x => x.ResourceTypeList)
        .WithOne(y => y.SearchParameter)
        .HasForeignKey(x => x.SearchParameterId);

        entity.HasMany(x => x.TargetResourceTypeList)
        .WithOne(y => y.SearchParameter)
        .HasForeignKey(x => x.SearchParameterId);

        entity.HasOne(x => x.SearchParamType)
        .WithMany()
        .HasForeignKey(x => x.SearchParamTypeId);

        entity.HasMany(x => x.ComponentList)
        .WithOne(y => y.SearchParameter)
        .HasForeignKey(x => x.SearchParameterId);

        entity.HasOne(x => x.FhirVersion)
        .WithMany()
        .HasForeignKey(x => x.FhirVersionId);

        entity.HasIndex(x => x.Name)
          .HasName("Ix_SearchParameter_Url").IsUnique(false);

      });

      if (GenerateNonStaticSeedData)
      {
        //builder.Entity<SearchParameter>().HasData(Bug.Data.Seeding.SearchParameterSeed.GetSeedData(DateTimeNow));
      }

      //##### ServiceBaseUrl #################################################
      builder.Entity<Bug.Logic.DomainModel.ServiceBaseUrl>(entity =>
      {
        SetupBaseIntKeyProperties(entity);
        entity.Property(x => x.Url).HasColumnName("url").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);        
        entity.Property(x => x.IsPrimary).HasColumnName("is_primary").IsRequired(true);
        entity.Property(x => x.FhirVersionId).HasColumnName("fhirversion_id").IsRequired(true).HasConversion<int>();
        
        entity.HasIndex(x => new { x.Url, x.FhirVersionId })
          .HasName("Ix_ServiceBaseUrl_Url_FhirVersionId")
          .IsUnique();
      });

      //##### IndexReference #################################################
      builder.Entity<Bug.Logic.DomainModel.IndexReference>(entity =>
      {
        SetupIndexBase(entity);

        entity.HasOne(x => x.ResourceStore)
          .WithMany(z => z.ReferenceIndexList)
          .HasForeignKey(x => x.ResourceStoreId);

        entity.Property(e => e.ServiceBaseUrlId).HasColumnName("servicebaseurl_id").IsRequired(true);
        entity.Property(e => e.ResourceTypeId).HasColumnName("resourcetype_id").IsRequired(true).HasConversion<int>();
        entity.Property(e => e.ResourceId).HasColumnName("resource_id").IsRequired(true);
        entity.Property(e => e.VersionId).HasColumnName("version_id").IsRequired(false);
        entity.Property(e => e.CanonicalVersionId).HasColumnName("canonical_version_id").IsRequired(false);

        entity.HasOne(x => x.ServiceBaseUrl)
        .WithMany()
        .HasForeignKey(x => x.ServiceBaseUrlId);

        entity.HasIndex(x => x.ResourceId)
          .HasName("Ix_IndexReference_ResourceId");

        entity.HasIndex(x => x.VersionId)
          .HasName("Ix_IndexReference_VersionId");

        entity.HasIndex(x => x.CanonicalVersionId)
         .HasName("Ix_IndexReference_CanonicalVersionId");
      });

      //##### IndexDateTime #################################################
      builder.Entity<Bug.Logic.DomainModel.IndexDateTime>(entity =>
      {
        SetupIndexBase(entity);

        entity.HasOne(x => x.ResourceStore)
         .WithMany(z => z.DateTimeIndexList)
         .HasForeignKey(x => x.ResourceStoreId);

        entity.Property(e => e.Low).HasColumnName("low").IsRequired(false);
        entity.Property(e => e.High).HasColumnName("high").IsRequired(false);

        entity.HasIndex(x => x.Low)
          .HasName("Ix_IndexDateTime_Low");

        entity.HasIndex(x => x.High)
          .HasName("Ix_IndexDateTime_High");
      });



      //##### IndexQuantity #################################################
      builder.Entity<Bug.Logic.DomainModel.IndexQuantity>(entity =>
      {
        SetupIndexBase(entity);

        entity.HasOne(x => x.ResourceStore)
          .WithMany(z => z.QuantityIndexList)
          .HasForeignKey(x => x.ResourceStoreId);

        entity.Property(e => e.System).HasColumnName("system").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);
        entity.Property(e => e.Code).HasColumnName("code").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.CodeMaxLength);
        entity.Property(e => e.Comparator).HasColumnName("comparator").IsRequired(false);
        //entity.Property(e => e.Quantity).HasColumnType($"DECIMAL ({DatabaseMetaData.FieldLength.QuantityPrecision}, {DatabaseMetaData.FieldLength.QuantityScale})");
        entity.Property(e => e.Quantity).HasColumnName("quantity").IsRequired(false);
        entity.Property(e => e.Unit).HasColumnName("unit").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);

        entity.Property(e => e.SystemHigh).HasColumnName("system_high").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);
        entity.Property(e => e.CodeHigh).HasColumnName("code_high").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.CodeMaxLength);
        entity.Property(e => e.ComparatorHigh).HasColumnName("comparator_high").IsRequired(false);
        //entity.Property(e => e.Quantity).HasColumnType($"DECIMAL ({DatabaseMetaData.FieldLength.QuantityPrecision}, {DatabaseMetaData.FieldLength.QuantityScale})");
        entity.Property(e => e.QuantityHigh).HasColumnName("quantity_high").IsRequired(false);
        entity.Property(e => e.UnitHigh).HasColumnName("unit_high").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);


        entity.HasIndex(x => x.Code)
          .HasName("Ix_IndexQuantity_Low");

        entity.HasIndex(x => x.System)
          .HasName("Ix_IndexQuantity_System");

        entity.HasIndex(x => x.Quantity)
          .HasName("Ix_IndexQuantity_Quantity");

        entity.HasIndex(x => x.CodeHigh)
          .HasName("Ix_IndexQuantity_CodeHigh");

        entity.HasIndex(x => x.SystemHigh)
          .HasName("Ix_IndexQuantity_SystemHigh");

        entity.HasIndex(x => x.QuantityHigh)
          .HasName("Ix_IndexQuantity_QuantityHigh");
      });

      //##### IndexQuantity #################################################
      builder.Entity<Bug.Logic.DomainModel.IndexString>(entity =>
      {
        SetupIndexBase(entity);

        entity.HasOne(x => x.ResourceStore)
          .WithMany(z => z.StringIndexList)
          .HasForeignKey(x => x.ResourceStoreId);

        entity.Property(e => e.String).HasColumnName("string").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);

        entity.HasIndex(x => x.String)
              .HasName("Ix_IndexString_String");

      });

      //##### IndexToken #################################################
      builder.Entity<Bug.Logic.DomainModel.IndexToken>(entity =>
      {
        SetupIndexBase(entity);

        entity.HasOne(x => x.ResourceStore)
          .WithMany(z => z.TokenIndexList)
          .HasForeignKey(x => x.ResourceStoreId);

        entity.Property(e => e.System).HasColumnName("system").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);
        entity.Property(e => e.Code).HasColumnName("code").IsRequired(false).HasMaxLength(DatabaseMetaData.FieldLength.CodeMaxLength);

        entity.HasIndex(x => x.System)
              .HasName("Ix_IndexToken_System");

        entity.HasIndex(x => x.Code)
              .HasName("Ix_IndexToken_Code");
      });

      //##### IndexUri #################################################
      builder.Entity<Bug.Logic.DomainModel.IndexUri>(entity =>
      {
        SetupIndexBase(entity);

        entity.HasOne(x => x.ResourceStore)
          .WithMany(z => z.UriIndexList)
          .HasForeignKey(x => x.ResourceStoreId);

        entity.Property(e => e.Uri).HasColumnName("uri").IsRequired(true).HasMaxLength(DatabaseMetaData.FieldLength.StringMaxLength);      

        entity.HasIndex(x => x.Uri)
              .HasName("Ix_IndexUri_Uri");
      });
    }

    private static void SetupIndexBase<T>(EntityTypeBuilder<T> entity) where T : IndexBase
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).HasColumnName("id");
      entity.Property(e => e.ResourceStoreId).HasColumnName("resourcestore_id").IsRequired(true);
      entity.Property(e => e.SearchParameterId).HasColumnName("searchparameter_id").IsRequired(true);

      entity.HasOne(x => x.SearchParameter)
      .WithMany()
      .HasForeignKey(x => x.SearchParameterId);
    }

    private static void SetupBaseIntKeyProperties<T>(EntityTypeBuilder<T> entity) where T : BaseIntKey
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
