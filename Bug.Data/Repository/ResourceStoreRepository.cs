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
        x.FkFhirVersionId == fhirMajorVersion & 
        x.FkResourceTypeId == resourceType & 
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
        FkFhirVersionId = x.FkFhirVersionId,
        FkResourceTypeId = x.FkResourceTypeId,
        ResourceType = x.ResourceType,
        FkMethodId = x.FkMethodId,
        FkHttpStatusCodeId = x.FkHttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FkFhirVersionId == fhirMajorVersion &
        y.FkResourceTypeId == resourceType).OrderBy(z => z.LastUpdated).ToListAsync();
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
        FkFhirVersionId = x.FkFhirVersionId,
        FkResourceTypeId = x.FkResourceTypeId,
        ResourceType = x.ResourceType,
        FkMethodId = x.FkMethodId,
        FkHttpStatusCodeId = x.FkHttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FkFhirVersionId == fhirMajorVersion).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task<ResourceStore?> GetVersionAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId, int versionId)
    {
      return await DbSet.SingleOrDefaultAsync(x =>
        x.FkFhirVersionId == fhirMajorVersion &
        x.FkResourceTypeId == resourceType &
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
        FkFhirVersionId = x.FkFhirVersionId,
        FkResourceTypeId = x.FkResourceTypeId,
        ResourceType = x.ResourceType,
        FkMethodId = x.FkMethodId,        
        FkHttpStatusCodeId = x.FkHttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FkFhirVersionId == fhirMajorVersion &
        y.FkResourceTypeId == resourceType &
        y.ResourceId == resourceId).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public void UpdateIsCurrent(ResourceStore resourceStore)
    {
      DbSet.Attach(resourceStore);
      _context.Entry(resourceStore).Property("IsCurrent").IsModified = true;
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
        FkFhirVersionId = x.FkFhirVersionId,
        FkResourceTypeId = x.FkResourceTypeId,
        ResourceType = x.ResourceType
      }).SingleOrDefaultAsync(y => 
        y.FkFhirVersionId == fhirMajorVersion &
        y.FkResourceTypeId == resourceType & 
        y.ResourceId == resourceId &
        y.IsCurrent == true);
    }

  }
}

