using Bug.Common.Enums;
using Bug.Logic.Service.SearchQuery.Tools;
using System.Threading.Tasks;

namespace Bug.Logic.Service.SearchQuery
{
  public interface ISearchQueryService
  {
    Task<SerachQueryServiceOutcome> Process(FhirVersion fhirVersion, ResourceType resourceTypeContext, IFhirSearchQuery searchQuery);
  }
}