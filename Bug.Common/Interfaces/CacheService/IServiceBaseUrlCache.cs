using Bug.Common.Interfaces.DomainModel;
using System.Threading.Tasks;

namespace Bug.Common.Interfaces.CacheService
{
  public interface IServiceBaseUrlCache
  {
    Task<IServiceBaseUrl?> GetAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url);
    Task<IServiceBaseUrl> GetPrimaryAsync(Bug.Common.Enums.FhirVersion fhirVersion);
    Task RemoveAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url);
    Task RemovePrimaryAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url);
    Task SetPrimaryAsync(IServiceBaseUrl serviceBaseUrl);
    Task SetAsync(IServiceBaseUrl serviceBaseUrl);
  }
}