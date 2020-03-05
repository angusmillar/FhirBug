using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.CacheService
{
  public interface IFhirVersionCache
  {
    Task<DomainModel.FhirVersion?> GetAsync(Common.Enums.FhirVersion fhirMajorVersion);
    Task RemoveAsync(Common.Enums.FhirVersion fhirMajorVersion);
    Task SetAsync(DomainModel.FhirVersion fhirVersion);
  }
}