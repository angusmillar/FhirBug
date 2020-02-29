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

  public class MethodCache : IMethodCache
  {
    private readonly IDistributedCache IDistributedCache;
    private readonly IMethodRepository IMethodRepository;
    private readonly IFhirServerConfig IFhirServerConfig;
    private const string ParameterName = "table:method:code:";
    public MethodCache(IDistributedCache IDistributedCache,
      IMethodRepository IMethodRepository,
      IFhirServerConfig IFhirServerConfig)
    {
      this.IDistributedCache = IDistributedCache;
      this.IMethodRepository = IMethodRepository;
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public async Task<Method?> GetAsync(HttpVerb httpVerb)
    {
      byte[]? data = await IDistributedCache.GetAsync($"{ParameterName}{httpVerb}");
      if (data is object)
      {
        return JsonSerializer.Deserialize<Method>(data);
      }
      else
      {
        Method? Method = await IMethodRepository.GetBy(httpVerb);
        if (Method is object)
        {
          await this.SetAsync(Method);
          return Method;
        }
        return null;
      }
    }

    public async Task SetAsync(Method method)
    {
      byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(method);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync($"{ParameterName}{method.HttpVerb}", jsonUtf8Bytes, RedisOptions);
    }

    public async Task RemoveAsync(HttpVerb httpVerb)
    {
      await IDistributedCache.RemoveAsync($"{ParameterName}{httpVerb}");
    }
  }
}
