using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexDateTimePredicateFactory
  {
    List<Expression<Func<IndexDateTime, bool>>> DateTimeIndex(SearchQueryDateTime SearchQueryDateTime);
  }
}