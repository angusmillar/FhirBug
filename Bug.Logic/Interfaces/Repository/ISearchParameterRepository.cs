using Bug.Logic.DomainModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface ISearchParameterRepository
  {
    Task<List<SearchParameter>> GetByAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
    Task<SearchParameter> GetByAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, string name);
    Task<List<SearchParameter>> GetForIndexingAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
  }
}