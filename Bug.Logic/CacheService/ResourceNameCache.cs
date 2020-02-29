using Bug.Common.ApplicationConfig;
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

  public class ResourceNameCache : IResourceNameCache
  {
    private readonly IDistributedCache IDistributedCache;
    private readonly IResourceNameRepository IResourceNameRepository;
    private readonly IFhirServerConfig IFhirServerConfig;
    private const string ParameterName = "table:resourceName:resourceName:";
    public ResourceNameCache(IDistributedCache IDistributedCache, 
      IResourceNameRepository IResourceNameRepository, 
      IFhirServerConfig IFhirServerConfig)
    {
      this.IDistributedCache = IDistributedCache;
      this.IResourceNameRepository = IResourceNameRepository;
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public async Task<ResourceName?> GetAsync(string ResourceName)
    {
      byte[]? data = await IDistributedCache.GetAsync($"{ParameterName}{ResourceName}");
      if (data is object)
      {
        return JsonSerializer.Deserialize<ResourceName>(data);
      }
      else
      {
        ResourceName? Resource = await IResourceNameRepository.GetByResourceName(ResourceName);
        if (Resource is object)
        {
          await this.SetAsync(Resource);
          return Resource;
        }                
        return null;
      }
    }

    public async Task SetAsync(ResourceName resourceName)
    {           
      byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(resourceName);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync($"{ParameterName}{resourceName.Name}", jsonUtf8Bytes, RedisOptions);
    }

    public async Task RemoveAsync(string ResourceName)
    {
      await IDistributedCache.RemoveAsync($"{ParameterName}{ResourceName}");      
    }
  }
}
