using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public interface IResourceStorePredicateFactory
  {
    Expression<Func<ResourceStore, bool>> CurrentMainResource(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
    Expression<Func<ResourceStore, bool>> NumberIndex(ISearchQueryBase SearchQueryBase);
    Expression<Func<ResourceStore, bool>> QuantityIndex(ISearchQueryBase SearchQueryBase);
    Task<Expression<Func<ResourceStore, bool>>> ReferenceIndex(ISearchQueryBase SearchQueryBase);    
    Expression<Func<ResourceStore, bool>> StringIndex(ISearchQueryBase SearchQueryBase);
    Expression<Func<ResourceStore, bool>> TokenIndex(ISearchQueryBase SearchQueryBase);
    Expression<Func<ResourceStore, bool>> UriIndex(ISearchQueryBase SearchQueryBase);
    Expression<Func<ResourceStore, bool>> DateTimeIndex(ISearchQueryBase SearchQueryBase);
    Task<Expression<Func<ResourceStore, bool>>> CompositeIndex(IPredicateFactory IPredicateFactory, ISearchQueryBase SearchQueryBase);
  }
}