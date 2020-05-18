using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public interface IPredicateFactory
  {
    ExpressionStarter<ResourceStore> CurrentMainResource(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
    Task<ExpressionStarter<ResourceStore>> GetIndexPredicate(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, IList<ISearchQueryBase> SearchQueryList);
    Task<IQueryable<ResourceStore>> ChainEntry(AppDbContext AppDbContext, Common.Enums.ResourceType ParentResourceTypeContext, IList<ISearchQueryBase> SearchQueryList);
  }
}