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
  public class SearchParameterRepository : Repository<SearchParameter>
  {
    public SearchParameterRepository(AppDbContext context)
      : base(context) { }

    //public async Task<HttpStatusCode?> GetBy(Common.Enums.FhirVersion fhirVersion, string resourceName, string name)
    //{
    //  return await DbSet.Where(x => x.FkFhirVersionId == fhirVersion & x.Name == name & x.ResourceNameList.Contains(y => y.ResourceName.));
    //}

  }
}

