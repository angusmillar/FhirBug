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
      //We synchronously get both the requested ResourceType and the Resource ResourceType from Redis       
      Task<byte[]?> ResourceResourceTask = IDistributedCache.GetAsync(GetCacheKey(fhirVersion, Common.Enums.ResourceType.Resource));
      Task<byte[]?> RequestedResourceTypeTask = IDistributedCache.GetAsync(GetCacheKey(fhirVersion, resourceType));
      await Task.WhenAll(ResourceResourceTask, RequestedResourceTypeTask);

      //If the requested ResourceType was not found then get from the database
      List<SearchParameter> RequestedResourceTypeSearchParameterList;
      if (RequestedResourceTypeTask.Result is object)
      {
        RequestedResourceTypeSearchParameterList = JsonSerializer.Deserialize<List<SearchParameter>>(RequestedResourceTypeTask.Result);
      }
      else
      {
        RequestedResourceTypeSearchParameterList = await ISearchParameterRepository.GetForIndexingAsync(fhirVersion, resourceType);
        if (RequestedResourceTypeSearchParameterList.Count > 0)
        {
          await this.SetForIndexingAsync(fhirVersion, resourceType, RequestedResourceTypeSearchParameterList);
        }        
      }

      //If the Resource ResourceType was not found then get from the database
      List<SearchParameter> ResourceResourceTypeSearchParameterList;
      if (ResourceResourceTask.Result is object)
      {
        ResourceResourceTypeSearchParameterList = JsonSerializer.Deserialize<List<SearchParameter>>(ResourceResourceTask.Result);
      }
      else
      {
        ResourceResourceTypeSearchParameterList = await ISearchParameterRepository.GetForIndexingAsync(fhirVersion, Common.Enums.ResourceType.Resource);
        if (ResourceResourceTypeSearchParameterList.Count > 0)
        {
          await this.SetForIndexingAsync(fhirVersion, Common.Enums.ResourceType.Resource, ResourceResourceTypeSearchParameterList);          
        }        
      }
      //Combine the two lists into one
      RequestedResourceTypeSearchParameterList.AddRange(ResourceResourceTypeSearchParameterList);
      return RequestedResourceTypeSearchParameterList;
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
