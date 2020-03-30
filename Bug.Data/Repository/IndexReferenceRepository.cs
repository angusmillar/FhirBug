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
using Bug.Logic.DomainModel.Projection;
//using System.Data.Entity;

namespace Bug.Data.Repository
{
  public class IndexReferenceRepository : Repository<IndexReference>, IIndexReferenceRepository
  {
    public IndexReferenceRepository(AppDbContext context)
      : base(context) { }

    public async Task<bool> AnyAsync(Common.Enums.FhirVersion fhirVersion, int serviceBaseUrlId, Common.Enums.ResourceType resourceTypeId, string resourceId, string? versionId = null)
    {
      return await DbSet.AnyAsync(x =>
        x.ResourceStore.FhirVersionId == fhirVersion &
        x.ServiceBaseUrlId == serviceBaseUrlId &
        x.ResourceTypeId == resourceTypeId &
        x.ResourceId == resourceId &
        x.VersionId == versionId);
        
    }

    public async Task<List<ReferentialIntegrityQuery>> GetResourcesReferenced(Common.Enums.FhirVersion fhirVersion, int serviceBaseUrlId, Common.Enums.ResourceType resourceTypeId, string resourceId, string? versionId = null)
    {
      return await DbSet.Select(x => new ReferentialIntegrityQuery()
      {
        TargetResourceId = x.ResourceStore.ResourceId,
        TargetResourceTypeId = x.ResourceStore.ResourceTypeId,
        FhirVersionId = x.ResourceStore.FhirVersionId,
        ServiceBaseUrlId = x.ServiceBaseUrlId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceId = x.ResourceId,
        VersionId = x.VersionId        
      }).Distinct().Take(100).Where(x =>
       x.FhirVersionId == fhirVersion &
       x.ServiceBaseUrlId == serviceBaseUrlId &
       x.ResourceTypeId == resourceTypeId &
       x.ResourceId == resourceId &
       x.VersionId == versionId).ToListAsync();      
    }
  }
}

