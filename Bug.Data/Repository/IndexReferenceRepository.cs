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

  }
}

