using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexStringPredicateFactory
  {
    Expression<Func<ResourceStore, bool>> StringIndex(SearchQueryString SearchQueryString);
  }
}