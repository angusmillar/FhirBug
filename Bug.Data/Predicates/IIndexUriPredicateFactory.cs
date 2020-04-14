using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;

namespace Bug.Data.Predicates
{
  public interface IIndexUriPredicateFactory
  {
    Expression<Func<ResourceStore, bool>> UriIndex(SearchQueryUri SearchQueryUri);
  }
}