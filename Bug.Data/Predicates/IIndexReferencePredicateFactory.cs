using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public interface IIndexReferencePredicateFactory
  {
    Task<Expression<Func<ResourceStore, bool>>> ReferenceIndex(SearchQueryReference SearchQueryReference);
  }
}