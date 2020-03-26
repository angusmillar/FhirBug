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
  public class IndexStringRepository : Repository<IndexString>
  {
    public IndexStringRepository(AppDbContext context)
      : base(context) { }

    public async Task RemoveIndexList(Common.Enums.FhirVersion fhirVersion, int resourceStoreId)
    {
      await DbSet.Where(x => x.ResourceStoreId == resourceStoreId)
        .Select(y => new IndexString() { Id = y.Id })
        .ForEachAsync(b => DbSet.Remove(b));
    }

  }
}

