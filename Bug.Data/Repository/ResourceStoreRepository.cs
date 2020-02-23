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
//using System.Data.Entity;

namespace Bug.Data.Repository
{
  public class ResourceStoreRepository : Repository<ResourceStore>, IResourceStoreRepository
  {
    public ResourceStoreRepository(AppDbContext context)
      : base(context) { }
     
    public async Task<ResourceStore> GetCurrentAsync(string fhirId)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.ResourceId == fhirId & x.IsCurrent == true);
    }

    public async Task<ResourceStore> GetCurrentNoBlobAsync(string fhirId)
    {
      return await DbSet.Select(x => new ResourceStore
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId
      }).SingleOrDefaultAsync(y => y.ResourceId == fhirId & y.IsCurrent == true);





    }

  }
}

