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
  public class FhirVersionRepository : Repository<FhirVersion>, IFhirVersionRepository
  {
    public FhirVersionRepository(AppDbContext context)
      : base(context) { }

    public async Task<FhirVersion?> GetByVersionAsycn(FhirMajorVersion fhirMajorVersion)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.FhirMajorVersion == fhirMajorVersion);
    }

  }
}

