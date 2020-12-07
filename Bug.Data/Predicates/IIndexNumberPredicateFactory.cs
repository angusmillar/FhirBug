using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexNumberPredicateFactory
  {
    List<Expression<Func<IndexQuantity, bool>>> NumberIndex(SearchQueryNumber SearchQueryNumber);
  }
}