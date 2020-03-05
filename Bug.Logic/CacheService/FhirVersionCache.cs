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

  public class FhirVersionCache : IFhirVersionCache
  {
    private readonly IDistributedCache IDistributedCache;
    private readonly IFhirVersionRepository IFhirVersionRepository;
    private readonly IFhirServerConfig IFhirServerConfig;
    private const string ParameterName = "table:fhirVersion:code:";
    public FhirVersionCache(IDistributedCache IDistributedCache,
      IFhirVersionRepository IFhirVersionRepository,
      IFhirServerConfig IFhirServerConfig)
    {
      this.IDistributedCache = IDistributedCache;
      this.IFhirVersionRepository = IFhirVersionRepository;
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public async Task<DomainModel.FhirVersion?> GetAsync(Common.Enums.FhirVersion fhirMajorVersion)
    {
      byte[]? data = await IDistributedCache.GetAsync($"{ParameterName}{fhirMajorVersion.GetCode()}");
      if (data is object)
      {
        return JsonSerializer.Deserialize<DomainModel.FhirVersion>(data);
      }
      else
      {
        DomainModel.FhirVersion? FhirVersion = await IFhirVersionRepository.GetByVersionAsycn(fhirMajorVersion);
        if (FhirVersion is object)
        {
          await this.SetAsync(FhirVersion);
          return FhirVersion;
        }
        return null;
      }
    }

    public async Task SetAsync(DomainModel.FhirVersion fhirVersion)
    {      
      byte[] jsonUtf8Bytes;
      jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(fhirVersion);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync($"{ParameterName}{fhirVersion.Id.GetCode()}", jsonUtf8Bytes, RedisOptions);      
    }

    public async Task RemoveAsync(Common.Enums.FhirVersion fhirMajorVersion)
    {
      await IDistributedCache.RemoveAsync($"{ParameterName}{fhirMajorVersion.GetCode()}");
    }
  }
}
