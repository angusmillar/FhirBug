using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bug.Data.Repository.Base;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
//using System.Data.Entity;

namespace Bug.Data.Repository
{
  public class ResourceStoreRepository : Repository<ResourceStore>, IResourceStoreRepository
  {
    public ResourceStoreRepository(AppDbContext context)
      : base(context) { }

    public ResourceStore GetByFhirId(string fhirId)
    {
      return DbSet.FirstOrDefault(x => x.FhirId == fhirId);
    }
    
  }
}

