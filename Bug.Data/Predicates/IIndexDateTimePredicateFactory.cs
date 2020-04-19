using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexDateTimePredicateFactory
  {
    Expression<Func<ResourceStore, bool>> DateTimeIndex(SearchQueryDateTime SearchQueryDateTime);
  }
}