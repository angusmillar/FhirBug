using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public interface IResourceStorePredicateFactory
  {
    Expression<Func<ResourceStore, bool>> CurrentMainResource(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType);
    List<Expression<Func<IndexQuantity, bool>>> NumberIndex(ISearchQueryBase SearchQueryBase);
    List<Expression<Func<IndexQuantity, bool>>> QuantityIndex(ISearchQueryBase SearchQueryBase);
    Task<List<Expression<Func<IndexReference, bool>>>> ReferenceIndex(ISearchQueryBase SearchQueryBase);
    List<Expression<Func<IndexString, bool>>> StringIndex(ISearchQueryBase SearchQueryBase);
    List<Expression<Func<IndexToken, bool>>> TokenIndex(ISearchQueryBase SearchQueryBase);
    List<Expression<Func<IndexUri, bool>>> UriIndex(ISearchQueryBase SearchQueryBase);
    List<Expression<Func<IndexDateTime, bool>>> DateTimeIndex(ISearchQueryBase SearchQueryBase);
    Task<Expression<Func<ResourceStore, bool>>> CompositeIndex(IPredicateFactory IPredicateFactory, ISearchQueryBase SearchQueryBase);
  }
}