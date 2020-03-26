using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bug.Data.Repository.Base;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System.Threading.Tasks;
using Bug.Common.Enums;
//using System.Data.Entity;

namespace Bug.Data.Repository
{
  public class ResourceStoreRepository : Repository<ResourceStore>, IResourceStoreRepository
  {
    public ResourceStoreRepository(AppDbContext context)
      : base(context) { }

    public async Task<ResourceStore?> GetCurrentAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId)
    {
      return await DbSet.SingleOrDefaultAsync(x =>
        x.FhirVersionId == fhirMajorVersion &
        x.ResourceTypeId == resourceType &
        x.ResourceId == resourceId &
        x.IsCurrent == true);
    }

    public async Task<IList<ResourceStore>> GetResourceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        ResourceBlob = x.ResourceBlob,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceType = x.ResourceType,
        MethodId = x.MethodId,
        HttpStatusCodeId = x.HttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FhirVersionId == fhirMajorVersion &
        y.ResourceTypeId == resourceType).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task<IList<ResourceStore>> GetBaseHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        ResourceBlob = x.ResourceBlob,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceType = x.ResourceType,
        MethodId = x.MethodId,
        HttpStatusCodeId = x.HttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FhirVersionId == fhirMajorVersion).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task<ResourceStore?> GetVersionAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId, int versionId)
    {
      return await DbSet.SingleOrDefaultAsync(x =>
        x.FhirVersionId == fhirMajorVersion &
        x.ResourceTypeId == resourceType &
        x.ResourceId == resourceId &
        x.VersionId == versionId);
    }

    public async Task<IList<ResourceStore>> GetInstanceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        ResourceBlob = x.ResourceBlob,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceType = x.ResourceType,
        MethodId = x.MethodId,
        HttpStatusCodeId = x.HttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FhirVersionId == fhirMajorVersion &
        y.ResourceTypeId == resourceType &
        y.ResourceId == resourceId).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public void UpdateIsCurrent(ResourceStore resourceStore)
    {
      DbSet.Attach(resourceStore);
      _context.Entry(resourceStore).Property(v => v.IsCurrent).IsModified = true;
      _context.Entry(resourceStore).Property(v => v.Updated).IsModified = true;

      //Delete all the indexes for this ResourceStore entity
      _context.Set<IndexDateTime>().Where(x => x.ResourceStoreId == resourceStore.Id)
        .Select(y => new IndexDateTime() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexDateTime>().Remove(b));

      _context.Set<IndexQuantity>().Where(x => x.ResourceStoreId == resourceStore.Id)
        .Select(y => new IndexQuantity() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexQuantity>().Remove(b));

      _context.Set<IndexReference>().Where(x => x.ResourceStoreId == resourceStore.Id)
        .Select(y => new IndexReference() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexReference>().Remove(b));

      _context.Set<IndexString>().Where(x => x.ResourceStoreId == resourceStore.Id)
        .Select(y => new IndexString() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexString>().Remove(b));

      _context.Set<IndexToken>().Where(x => x.ResourceStoreId == resourceStore.Id)
        .Select(y => new IndexToken() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexToken>().Remove(b));

      _context.Set<IndexUri>().Where(x => x.ResourceStoreId == resourceStore.Id)
        .Select(y => new IndexUri() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexUri>().Remove(b));
    }



    public async Task<ResourceStore?> GetCurrentMetaAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceType = x.ResourceType
      }).SingleOrDefaultAsync(y =>
        y.FhirVersionId == fhirMajorVersion &
        y.ResourceTypeId == resourceType &
        y.ResourceId == resourceId &
        y.IsCurrent == true);
    }

  }
}

