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
  public class HttpStatusCodeCache : IHttpStatusCodeCache
  {
    private readonly IDistributedCache IDistributedCache;
    private readonly IHttpStatusCodeRepository IHttpStatusCodeRepository;
    private readonly IFhirServerConfig IFhirServerConfig;
    private const string ParameterName = "table:httpStatusCode:code:";
    public HttpStatusCodeCache(IDistributedCache IDistributedCache,
      IHttpStatusCodeRepository IHttpStatusCodeRepository,
      IFhirServerConfig IFhirServerConfig)
    {
      this.IDistributedCache = IDistributedCache;
      this.IHttpStatusCodeRepository = IHttpStatusCodeRepository;
      this.IFhirServerConfig = IFhirServerConfig;
    }

    public async Task<HttpStatusCode?> GetAsync(System.Net.HttpStatusCode httpStatusCode)
    {
      byte[]? data = await IDistributedCache.GetAsync($"{ParameterName}{httpStatusCode.ToString()}");
      if (data is object)
      {
        return JsonSerializer.Deserialize<HttpStatusCode>(data);
      }
      else
      {
        HttpStatusCode? HttpStatusCode = await IHttpStatusCodeRepository.GetByCode(httpStatusCode);
        if (HttpStatusCode is object)
        {
          await this.SetAsync(HttpStatusCode);
          return HttpStatusCode;
        }
        return null;
      }
    }

    public async Task SetAsync(HttpStatusCode httpStatusCode)
    {
      byte[] jsonUtf8Bytes;
      jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(httpStatusCode);
      var RedisOptions = new DistributedCacheEntryOptions()
      {
        SlidingExpiration = TimeSpan.FromMinutes(IFhirServerConfig.CahceSlidingExpirationMinites)
      };
      await IDistributedCache.SetAsync($"{ParameterName}{httpStatusCode.Code.ToString()}", jsonUtf8Bytes, RedisOptions);
    }

    public async Task RemoveAsync(System.Net.HttpStatusCode httpStatusCode)
    {
      await IDistributedCache.RemoveAsync($"{ParameterName}{httpStatusCode.ToString()}");
    }
  }
}
