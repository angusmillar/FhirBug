using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Bug.Logic.CacheService
{

  public class SearchParameterCache : ISearchParameterCache
  {
    private readonly IDistributedCache IDistributedCache;
    private readonly ISearchParameterRepository ISearchParameterRepository;
    private readonly IFhirServerConfig IFhirServerConfig;
    private const string ParameterName = "table:searchParameter:fhirVersion+resource:";
    public SearchParameterCache(IDistributedCache IDistributedCache,
      ISearchParameterRepository ISearchParameterRepository,
      IFhirServerConfig IFhirServerConfig)
    {
      this.IDistributedCache = IDistributedCache;
      this.ISearchParameterRepository = ISearchParameterRepository;
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public async Task<List<SearchParameter>> GetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      byte[]? data = await IDistributedCache.GetAsync(GetCacheKey(fhirVersion, resourceType));
      if (data is object)
      {
        return JsonSerializer.Deserialize<List<SearchParameter>>(data);
      }
      else
      {
        List<SearchParameter> SearchParameterList = await ISearchParameterRepository.GetForIndexingAsync(fhirVersion, resourceType);
        if (SearchParameterList.Count > 0)
        {
          await this.SetForIndexingAsync(fhirVersion, resourceType, SearchParameterList);
        }
        return SearchParameterList;
      }
    }

    public async Task SetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, List<SearchParameter> searchParameterList)
    {
      byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(searchParameterList);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync(GetCacheKey(fhirVersion, resourceType), jsonUtf8Bytes, RedisOptions);
    }

    public async Task RemoveForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      await IDistributedCache.RemoveAsync(GetCacheKey(fhirVersion, resourceType));
    }

    private string GetCacheKey(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      return $"{ParameterName}{((int)fhirVersion).ToString()}+{((int)resourceType).ToString()}";
    }
  }
}
