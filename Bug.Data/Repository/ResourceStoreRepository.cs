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
     
    public async Task<ResourceStore?> GetCurrentAsync(Common.Enums.FhirVersion fhirMajorVersion, string resourceName, string resourceId)
    {
      return await DbSet.SingleOrDefaultAsync(x => 
        x.FkFhirVersionId == fhirMajorVersion & 
        x.ResourceName.Name == resourceName & 
        x.ResourceId == resourceId & 
        x.IsCurrent == true);
    }

    public async Task<IList<ResourceStore>> GetResourceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, string resourceName)
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
        FkResourceNameId = x.FkResourceNameId,
        ResourceName = x.ResourceName,
        FkMethodId = x.FkMethodId,
        FkHttpStatusCodeId = x.FkHttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FkFhirVersionId == fhirMajorVersion &
        y.ResourceName.Name == resourceName).OrderBy(z => z.LastUpdated).ToListAsync();
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
        FkResourceNameId = x.FkResourceNameId,
        ResourceName = x.ResourceName,
        FkMethodId = x.FkMethodId,
        FkHttpStatusCodeId = x.FkHttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FkFhirVersionId == fhirMajorVersion).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task<ResourceStore?> GetVersionAsync(Common.Enums.FhirVersion fhirMajorVersion, string resourceName, string resourceId, int versionId)
    {
      return await DbSet.SingleOrDefaultAsync(x =>
        x.FkFhirVersionId == fhirMajorVersion &
        x.ResourceName.Name == resourceName &
        x.ResourceId == resourceId &
        x.VersionId == versionId);
    }

    public async Task<IList<ResourceStore>> GetInstanceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, string resourceName, string resourceId)
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
        FkResourceNameId = x.FkResourceNameId,
        ResourceName = x.ResourceName,
        FkMethodId = x.FkMethodId,        
        FkHttpStatusCodeId = x.FkHttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FkFhirVersionId == fhirMajorVersion &
        y.ResourceName.Name == resourceName &
        y.ResourceId == resourceId).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public void UpdateIsCurrent(ResourceStore resourceStore)
    {
      DbSet.Attach(resourceStore);
      _context.Entry(resourceStore).Property("IsCurrent").IsModified = true;
    }

    

    public async Task<ResourceStore?> GetCurrentMetaAsync(Common.Enums.FhirVersion fhirMajorVersion, string ResourceName, string resourceId)
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
        FkResourceNameId = x.FkResourceNameId,
        ResourceName = x.ResourceName
      }).SingleOrDefaultAsync(y => 
        y.FkFhirVersionId == fhirMajorVersion &
        y.ResourceName.Name == ResourceName & 
        y.ResourceId == resourceId &
        y.IsCurrent == true);
    }

  }
}

