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
using Bug.Common.StringTools;

namespace Bug.Logic.CacheService
{
  public class ServiceBaseUrlCache : IServiceBaseUrlCache
  {
    private readonly IDistributedCache IDistributedCache;
    private readonly IServiceBaseUrlRepository IServiceBaseUrlRepository;
    private readonly Bug.Common.ApplicationConfig.IServiceBaseUrlConfi IServiceBaseUrlConfig;
    private readonly Bug.Common.ApplicationConfig.IFhirServerConfig IFhirServerConfig;
    private const string ParameterNameUrl = "table:serviceBaseUrl:fhirVersion:url:";
    private const string ParameterNameIsPrimary = "table:serviceBaseUrl:fhirVersion:isPrimary::";
    public ServiceBaseUrlCache(IDistributedCache IDistributedCache,
      IServiceBaseUrlRepository IServiceBaseUrlRepository,
      Bug.Common.ApplicationConfig.IServiceBaseUrlConfi IServiceBaseUrlConfig,
      Bug.Common.ApplicationConfig.IFhirServerConfig IFhirServerConfig)
    {
      this.IDistributedCache = IDistributedCache;
      this.IServiceBaseUrlRepository = IServiceBaseUrlRepository;
      this.IServiceBaseUrlConfig = IServiceBaseUrlConfig;
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public async Task<IServiceBaseUrl?> GetAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url)
    {
      byte[]? data = await IDistributedCache.GetAsync(GetKey(fhirVersion, url));
      if (data is object)
      {
        return JsonSerializer.Deserialize<ServiceBaseUrl>(data);
      }
      else
      {
        IServiceBaseUrl? ServiceBaseUrl = await IServiceBaseUrlRepository.GetBy(fhirVersion, url);
        if (ServiceBaseUrl is object && ServiceBaseUrl is IServiceBaseUrl IServiceBaseUrl)
        {
          await this.SetAsync(IServiceBaseUrl);
          return IServiceBaseUrl;
        }
        return null;
      }
    }

    public async Task<IServiceBaseUrl> GetPrimaryAsync(Bug.Common.Enums.FhirVersion fhirVersion)
    {
      byte[]? data = await IDistributedCache.GetAsync(GetPrimaryKey(fhirVersion));
      if (data is object)
      {
        return JsonSerializer.Deserialize<ServiceBaseUrl>(data);
      }
      else
      {
        IServiceBaseUrl? ServiceBaseUrl = await IServiceBaseUrlRepository.GetPrimary(fhirVersion);
        if (ServiceBaseUrl is object)
        {
          await this.SetPrimaryAsync(ServiceBaseUrl);
          return ServiceBaseUrl;
        }
        else
        {
          ServiceBaseUrl = await IServiceBaseUrlRepository.AddAsync(fhirVersion, StringSupport.StripHttp(IServiceBaseUrlConfig.Url(fhirVersion).OriginalString), true);
          await this.SetPrimaryAsync(ServiceBaseUrl);
          return ServiceBaseUrl;
        }
      }
    }

    public async Task SetPrimaryAsync(IServiceBaseUrl serviceBaseUrl)
    {
      byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(serviceBaseUrl);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync(GetPrimaryKey(serviceBaseUrl.FhirVersionId), jsonUtf8Bytes, RedisOptions);
    }

    public async Task SetAsync(IServiceBaseUrl serviceBaseUrl)
    {
      byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(serviceBaseUrl);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync(GetKey(serviceBaseUrl.FhirVersionId, serviceBaseUrl.Url), jsonUtf8Bytes, RedisOptions);
    }

    public async Task RemoveAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url)
    {
      await IDistributedCache.RemoveAsync(GetKey(fhirVersion, url));
    }

    public async Task RemovePrimaryAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url)
    {
      await IDistributedCache.RemoveAsync(GetPrimaryKey(fhirVersion));
      await RemoveAsync(fhirVersion, url);
    }

    private string GetPrimaryKey(Bug.Common.Enums.FhirVersion fhirVersion)
    {
      return $"table:serviceBaseUrl:fVer:{((int)fhirVersion).ToString()}:isPrimary:true";
    }

    private string GetKey(Bug.Common.Enums.FhirVersion fhirVersion, string url)
    {
      return $"table:serviceBaseUrl:fVer:{((int)fhirVersion).ToString()}:url:{url}";
    }
  }
}
