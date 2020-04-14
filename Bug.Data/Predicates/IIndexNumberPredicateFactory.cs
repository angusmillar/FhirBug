using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexNumberPredicateFactory
  {
    Expression<Func<ResourceStore, bool>> NumberIndex(SearchQueryNumber SearchQueryNumber);
  }
}