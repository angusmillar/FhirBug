using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bug.Data.Repository.Base;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System.Threading.Tasks;
using Bug.Logic.DomainModel.Projection;
using Bug.Common.Enums;
//using System.Data.Entity;

namespace Bug.Data.Repository
{
  public class ResourceStoreRepository : Repository<ResourceStore>, IResourceStoreRepository
  {
    public ResourceStoreRepository(AppDbContext context)
      : base(context) { }
     
    public async Task<ResourceStore?> GetCurrentAsync(FhirMajorVersion fhirMajorVersion, string resourceName, string resourceId)
    {
      return await DbSet.SingleOrDefaultAsync(x => 
        x.FhirVersion.FhirMajorVersion == fhirMajorVersion & 
        x.ResourceName.Name == resourceName & 
        x.ResourceId == resourceId & 
        x.IsCurrent == true);
    }

    public async Task<ResourceStore?> GetVersionAsync(FhirMajorVersion fhirMajorVersion, string resourceName, string resourceId, int versionId)
    {
      return await DbSet.SingleOrDefaultAsync(x =>
        x.FhirVersion.FhirMajorVersion == fhirMajorVersion &
        x.ResourceName.Name == resourceName &
        x.ResourceId == resourceId &
        x.VersionId == versionId);
    }

    public void UpdateIsCurrent(ResourceStore resourceStore)
    {
      DbSet.Attach(resourceStore);
      _context.Entry(resourceStore).Property("IsCurrent").IsModified = true;
    }

    public async Task<ResourceStore?> GetCurrentMetaAsync(FhirMajorVersion fhirMajorVersion, string ResourceName, string resourceId)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        FhirVersion = x.FhirVersion,
        ResourceName = x.ResourceName
      }).SingleOrDefaultAsync(y => 
        y.FhirVersion.FhirMajorVersion == fhirMajorVersion &
        y.ResourceName.Name == ResourceName & 
        y.ResourceId == resourceId &
        y.IsCurrent == true);
    }

  }
}

