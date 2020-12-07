using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public interface IIndexReferencePredicateFactory
  {
    Task<List<Expression<Func<IndexReference, bool>>>> ReferenceIndex(SearchQueryReference SearchQueryReference);
  }
}