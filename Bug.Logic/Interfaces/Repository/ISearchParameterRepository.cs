using Bug.Logic.DomainModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface ISearchParameterRepository
  {
    Task<List<SearchParameter>> GetByResourceTypeAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
    Task<SearchParameter> GetByNameAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, string name);
    Task<SearchParameter?> GetByCanonicalUrlAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, string CanonicalUrl);
    Task<List<SearchParameter>> GetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
  }
}