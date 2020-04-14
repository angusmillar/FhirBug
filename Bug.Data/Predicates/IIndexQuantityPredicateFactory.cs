using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexQuantityPredicateFactory
  {
    Expression<Func<ResourceStore, bool>> QuantityIndex(SearchQueryQuantity SearchQueryQuantity);
  }
}