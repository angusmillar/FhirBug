using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bug.Data.Repository.Base;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System.Threading.Tasks;
//using System.Data.Entity;

namespace Bug.Data.Repository
{
  public class ResourceNameRepository : Repository<ResourceName>, IResourceNameRepository
  {
    public ResourceNameRepository(AppDbContext context)
      : base(context) { }

    public async Task<ResourceName?> GetByResourceName(string ResourceName)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.Name == ResourceName);
    }

  }
}

