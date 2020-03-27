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

    public async Task<bool> ReferentialIntegrityCheckAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceTypeId, string resourceId, int? versionId = null)
    {
      if (versionId.HasValue)
      {
        return await DbSet.AnyAsync(x =>
          x.FhirVersionId == fhirVersion &
          x.ResourceTypeId == resourceTypeId &
          x.ResourceId == resourceId &
          x.VersionId == versionId.Value &          
          x.IsDeleted == false);
      }
      else
      {
        return await DbSet.AnyAsync(x =>
          x.FhirVersionId == fhirVersion &
          x.ResourceTypeId == resourceTypeId &
          x.ResourceId == resourceId &
          x.IsCurrent == true &
          x.IsDeleted == false);
      }
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

    public void UpdateCurrent(ResourceStore resourceStore)
    {
      DbSet.Attach(resourceStore);
      _context.Entry(resourceStore).Property(v => v.IsCurrent).IsModified = true;
      _context.Entry(resourceStore).Property(v => v.Updated).IsModified = true;

      RemoveAllIndexes(resourceStore);
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
        //ResourceType = x.ResourceType
      }).SingleOrDefaultAsync(y =>
        y.FhirVersionId == fhirMajorVersion &
        y.ResourceTypeId == resourceType &
        y.ResourceId == resourceId &
        y.IsCurrent == true);
    }

    private void RemoveAllIndexes(ResourceStore resourceStore)
    {
      RemoveIndex<IndexDateTime>(resourceStore);
      RemoveIndex<IndexQuantity>(resourceStore);
      RemoveIndex<IndexReference>(resourceStore);
      RemoveIndex<IndexString>(resourceStore);
      RemoveIndex<IndexToken>(resourceStore);
      RemoveIndex<IndexUri>(resourceStore);
    }

    private void RemoveIndex<IndexType>(ResourceStore resourceStore) where IndexType : IndexBase, new()
    {
      _context.Set<IndexType>().Where(x => x.ResourceStoreId == resourceStore.Id)
        .Select(y => new IndexType() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexType>().Remove(b));
    }

  }
}

