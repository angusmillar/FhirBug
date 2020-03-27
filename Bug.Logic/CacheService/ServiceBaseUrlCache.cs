using Bug.Common.Enums;
using Bug.Common.Interfaces.DomainModel;
using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Bug.Common.Interfaces.Repository;

namespace Bug.Logic.CacheService
{
  public class ServiceBaseUrlCache : IServiceBaseUrlCache
  {
    private readonly IDistributedCache IDistributedCache;
    private readonly IServiceBaseUrlRepository IServiceBaseUrlRepository;
    private readonly Bug.Common.ApplicationConfig.IFhirServerConfig IFhirServerConfig;
    private const string ParameterNameUrl = "table:serviceBaseUrl:url:";
    private const string ParameterNameIsPrimary = "table:serviceBaseUrl:isPrimary:true";
    public ServiceBaseUrlCache(IDistributedCache IDistributedCache,
      IServiceBaseUrlRepository IServiceBaseUrlRepository,
      Bug.Common.ApplicationConfig.IFhirServerConfig IFhirServerConfig)
    {
      this.IDistributedCache = IDistributedCache;
      this.IServiceBaseUrlRepository = IServiceBaseUrlRepository;
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public async Task<IServiceBaseUrl?> GetAsync(string url)
    {
      byte[]? data = await IDistributedCache.GetAsync($"{ParameterNameUrl}{url}");
      if (data is object)
      {
        return JsonSerializer.Deserialize<ServiceBaseUrl>(data);
      }
      else
      {
       IServiceBaseUrl? ServiceBaseUrl = await IServiceBaseUrlRepository.GetBy(url);
        if (ServiceBaseUrl is object && ServiceBaseUrl is IServiceBaseUrl IServiceBaseUrl)
        {
          await this.SetAsync(IServiceBaseUrl);
          return IServiceBaseUrl;
        }
        return null;
      }
    }

    public async Task<IServiceBaseUrl> GetPrimaryAsync()
    {
      byte[]? data = await IDistributedCache.GetAsync($"{ParameterNameIsPrimary}");
      if (data is object)
      {
        return JsonSerializer.Deserialize<ServiceBaseUrl>(data);
      }
      else
      {
        IServiceBaseUrl ServiceBaseUrl = await IServiceBaseUrlRepository.GetPrimary();
        if (ServiceBaseUrl is IServiceBaseUrl IServiceBaseUrl)
        {
          await this.SetPrimaryAsync(IServiceBaseUrl);
          return IServiceBaseUrl;
        }
        return null;
      }
    }

    public async Task SetPrimaryAsync(IServiceBaseUrl serviceBaseUrl)
    {
      byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(serviceBaseUrl);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync($"{ParameterNameIsPrimary}", jsonUtf8Bytes, RedisOptions);
    }

    public async Task SetAsync(IServiceBaseUrl serviceBaseUrl)
    {
      byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(serviceBaseUrl);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync($"{ParameterNameUrl}{serviceBaseUrl.Url}", jsonUtf8Bytes, RedisOptions);
    }

    public async Task RemoveAsync(string url)
    {
      await IDistributedCache.RemoveAsync($"{ParameterNameUrl}{url}");
    }

    public async Task RemovePrimaryAsync(string url)
    {
      await IDistributedCache.RemoveAsync($"{ParameterNameIsPrimary}");
    }
  }
}
