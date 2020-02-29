using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.CacheService
{
  public interface IFhirVersionCache
  {
    Task<FhirVersion?> GetAsync(FhirMajorVersion fhirMajorVersion);
    Task RemoveAsync(FhirMajorVersion fhirMajorVersion);
    Task SetAsync(FhirVersion fhirVersion);
  }
}