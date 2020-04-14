using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexTokenPredicateFactory
  {
    Expression<Func<ResourceStore, bool>> TokenIndex(SearchQueryToken SearchQueryToken);
  }
}