using Bug.Logic.DomainModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.CacheService
{
  public interface ISearchParameterCache
  {
    Task<List<SearchParameter>> GetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
    Task RemoveForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
    Task SetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, List<SearchParameter> searchParameterList);
  }
}