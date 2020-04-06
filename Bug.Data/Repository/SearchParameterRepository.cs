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
      return await DbSet.SingleOrDefaultAsync(x => x.FhirVersionId == fhirVersion & x.Name == name & x.ResourceTypeList.Any(y => y.ResourceTypeId == resourceType));
    }

    public async Task<List<SearchParameter>> GetByAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      return await DbSet.Where(x => x.FhirVersionId == fhirVersion & x.ResourceTypeList.Any(y => y.ResourceTypeId == resourceType)).ToListAsync();
    }

    public async Task<List<SearchParameter>> GetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {

      return await DbSet
      .Include(d => d.TargetResourceTypeList)
      .Where(y => y.FhirVersionId == fhirVersion & y.ResourceTypeList.Any(z => z.ResourceTypeId == resourceType))
      .Select(x => new SearchParameter()
      {
        Id = x.Id,
        Name = x.Name,
        FhirPath = x.FhirPath,
        SearchParamTypeId = x.SearchParamTypeId,
        TargetResourceTypeList = x.TargetResourceTypeList,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeList = x.ResourceTypeList
      }).ToListAsync();

      //return await DbSet
      //.Include(d => d.TargetResourceTypeList)
      //.Where(y => y.FhirVersionId == fhirVersion & y.ResourceTypeList.Any(z => z.ResourceTypeId == resourceType))
      //.Select(x => new SearchParameter()
      //{ 
      //  Id = x.Id,
      //  Name = x.Name,
      //  FhirPath = x.FhirPath,
      //  SearchParamTypeId = x.SearchParamTypeId,        
      //  TargetResourceTypeList = x.TargetResourceTypeList,        
      //}).ToListAsync();
    }

    public async Task<List<SearchParameter>> GetForSearchQueryAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      return await DbSet
      .Include(d => d.TargetResourceTypeList)
      .Where(y => y.FhirVersionId == fhirVersion & y.ResourceTypeList.Any(z => z.ResourceTypeId == resourceType))
      .Select(x => new SearchParameter()
      {
        Id = x.Id,
        Name = x.Name,
        FhirPath = x.FhirPath,
        SearchParamTypeId = x.SearchParamTypeId,
        TargetResourceTypeList = x.TargetResourceTypeList,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeList = x.ResourceTypeList
      }).ToListAsync();
    }
  }
}

