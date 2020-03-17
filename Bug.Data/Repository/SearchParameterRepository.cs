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
  public class SearchParameterRepository : Repository<SearchParameter>, ISearchParameterRepository
  {
    public SearchParameterRepository(AppDbContext context)
      : base(context) { }

    public async Task<SearchParameter> GetByAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, string name)
    {
      return await DbSet.SingleOrDefaultAsync(x => x.FkFhirVersionId == fhirVersion & x.Name == name & x.ResourceTypeList.Any(y => y.FkResourceTypeId == resourceType));
    }

    public async Task<List<SearchParameter>> GetByAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      return await DbSet.Where(x => x.FkFhirVersionId == fhirVersion & x.ResourceTypeList.Any(y => y.FkResourceTypeId == resourceType)).ToListAsync();
    }

    public async Task<List<SearchParameter>> GetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      return await DbSet
      .Include(d => d.TargetResourceTypeList)
      .Where(y => y.FkFhirVersionId == fhirVersion & y.ResourceTypeList.Any(z => z.FkResourceTypeId == resourceType))
      .Select(x => new SearchParameter()
      { 
        Id = x.Id,
        Name = x.Name,
        FhirPath = x.FhirPath,
        FkSearchParamTypeId = x.FkSearchParamTypeId,        
        TargetResourceTypeList = x.TargetResourceTypeList,        
      }).ToListAsync();
    }
  }
}

