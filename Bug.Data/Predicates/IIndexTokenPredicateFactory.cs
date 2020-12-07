using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexTokenPredicateFactory
  {
    List<Expression<Func<IndexToken, bool>>> TokenIndex(SearchQueryToken SearchQueryToken);
  }
}