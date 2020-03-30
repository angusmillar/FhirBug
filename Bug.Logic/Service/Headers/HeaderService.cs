using Bug.Common.Enums;
using Bug.Common.Interfaces.CacheService;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Headers
{
  public class HeaderService : IHeaderService
  {
    private readonly IServiceBaseUrlCache IServiceBaseUrlCache;
    public HeaderService(IServiceBaseUrlCache IServiceBaseUrlCache)
    {
      this.IServiceBaseUrlCache = IServiceBaseUrlCache;
    }

    public void AddLastUpdated(Dictionary<string, StringValues> Headers, DateTimeOffset value)
    {
      Headers.Add(HeaderNames.LastModified, new StringValues(value.ToString("r", System.Globalization.CultureInfo.CurrentCulture)));
    }

    public void AddETag(Dictionary<string, StringValues> Headers, int versionId)
    {
      Headers.Add(HeaderNames.ETag, new StringValues($"W/\"{versionId.ToString()}\""));
    }

    public async Task AddLocationAsync(Dictionary<string, StringValues> Headers, FhirVersion fhirVersion, string scheme, string resourceId, int versionId)
    {
      var ServiceBaseUrl = await IServiceBaseUrlCache.GetPrimaryAsync(fhirVersion);
      Headers.Add(HeaderNames.Location, $"{scheme}://{ServiceBaseUrl.Url}/{resourceId}/_history/{versionId.ToString()}");
    }

    public async Task<Dictionary<string, StringValues>> AddForCreateAsync(FhirVersion fhirVersion, string requestScheme, DateTimeOffset lastUpdated, string resourceId, int versionId)
    {
      var Headers = new Dictionary<string, StringValues>();
      this.AddLastUpdated(Headers, lastUpdated);
      this.AddETag(Headers, versionId);
      await this.AddLocationAsync(Headers, fhirVersion, requestScheme, resourceId, versionId);
      return Headers;
    }

    public async Task<Dictionary<string, StringValues>> GetForUpdateAsync(FhirVersion fhirVersion, string requestScheme, DateTimeOffset lastUpdated, string resourceId, int versionId)
    {
      var Headers = new Dictionary<string, StringValues>();
      this.AddLastUpdated(Headers, lastUpdated);
      this.AddETag(Headers, versionId);
      //Only add the Location header is the update was a create, which would mean the version would be equal to 1 
      if (versionId == 1)
      {
        await this.AddLocationAsync(Headers, fhirVersion, requestScheme, resourceId, versionId);
      }
      return Headers;
    }

    public Dictionary<string, StringValues> GetForDelete(int? versionId = null)
    {
      var Headers = new Dictionary<string, StringValues>();
      if (versionId.HasValue)
      {
        this.AddETag(Headers, versionId.Value);        
      }
      return Headers;
    }
    public Dictionary<string, StringValues> GetForRead(DateTimeOffset lastUpdated, int versionId)
    {
      var Headers = new Dictionary<string, StringValues>();
      this.AddLastUpdated(Headers, lastUpdated);
      this.AddETag(Headers, versionId);
      return Headers;
    }
  }
}
