using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public interface IIndexCompositePredicateFactory
  {
    Task<Expression<Func<ResourceStore, bool>>> CompositeIndex(IPredicateFactory IPredicateFactory, SearchQueryComposite SearchQueryComposite);
  }
}