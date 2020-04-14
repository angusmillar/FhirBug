using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Data.Predicates
{
  public class PredicateFactory : IPredicateFactory
  {
    public ExpressionStarter<ResourceStore> Get(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, IList<ISearchQueryBase> SearchQueryList)
    {
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IEnumerable<ISearchQueryBase> NoChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is null);

      var CurrentMainResource = ResourceStorePredicateFactory.CurrentMainResource(fhirVersion, resourceType);
      var Predicate = PredicateBuilder.New<ResourceStore>(CurrentMainResource);

      foreach (var Search in NoChainedSearchQueryList)
      {
        switch (Search.SearchParamTypeId)
        {
          case Common.Enums.SearchParamType.Number:
            Predicate = Predicate.Extend(ResourceStorePredicateFactory.NumberIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Date:
            break;
          case Common.Enums.SearchParamType.String:
            Predicate = Predicate.Extend(ResourceStorePredicateFactory.StringIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Token:
            Predicate = Predicate.Extend(ResourceStorePredicateFactory.TokenIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Reference:
            break;
          case Common.Enums.SearchParamType.Composite:
            break;
          case Common.Enums.SearchParamType.Quantity:
            Predicate = Predicate.Extend(ResourceStorePredicateFactory.QuantityIndex(Search), PredicateOperator.And);           
            break;
          case Common.Enums.SearchParamType.Uri:
            Predicate = Predicate.Extend(ResourceStorePredicateFactory.UriIndex(Search), PredicateOperator.And);            
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
