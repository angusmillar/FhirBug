using Bug.Common.Enums;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Headers
{
  public interface IHeaderService
  {
    Task<Dictionary<string, StringValues>> AddForCreateAsync(FhirVersion fhirVersion, string requestScheme, DateTimeOffset lastUpdated, string resourceId, int versionId);
    Task<Dictionary<string, StringValues>> GetForUpdateAsync(FhirVersion fhirVersion, string requestScheme, DateTimeOffset lastUpdated, string resourceId, int versionId);
    Dictionary<string, StringValues> GetForRead(DateTimeOffset lastUpdated, int versionId);
    Dictionary<string, StringValues> GetForDelete(int? versionId = null);
    void AddETag(Dictionary<string, StringValues> Headers, int versionId);
    void AddLastUpdated(Dictionary<string, StringValues> Headers, DateTimeOffset value);
    Task AddLocationAsync(Dictionary<string, StringValues> Headers, FhirVersion fhirVersion, string scheme, string resourceId, int versionId);
    
  }
}