using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public class PredicateFactory : IPredicateFactory
  {
    private readonly IResourceStorePredicateFactory IResourceStorePredicateFactory;
    public PredicateFactory(IResourceStorePredicateFactory IResourceStorePredicateFactory)
    {
      this.IResourceStorePredicateFactory = IResourceStorePredicateFactory;
    }

    public async Task<ExpressionStarter<ResourceStore>> Get(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, IList<ISearchQueryBase> SearchQueryList)
    {
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IEnumerable<ISearchQueryBase> NoChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is null);

      var CurrentMainResource = IResourceStorePredicateFactory.CurrentMainResource(fhirVersion, resourceType);
      var Predicate = PredicateBuilder.New<ResourceStore>(CurrentMainResource);

      foreach (var Search in NoChainedSearchQueryList)
      {
        switch (Search.SearchParamTypeId)
        {
          case Common.Enums.SearchParamType.Number:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.NumberIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Date:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.DateTimeIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.String:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.StringIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Token:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.TokenIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Reference:            
            Predicate = Predicate.Extend(await IResourceStorePredicateFactory.ReferenceIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Composite:
            break;
          case Common.Enums.SearchParamType.Quantity:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.QuantityIndex(Search), PredicateOperator.And);           
            break;
          case Common.Enums.SearchParamType.Uri:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.UriIndex(Search), PredicateOperator.And);            
            break;
          case Common.Enums.SearchParamType.Special:
            break;
          default:
            break;
        }
      }
      return Predicate;
    }
  }
}
