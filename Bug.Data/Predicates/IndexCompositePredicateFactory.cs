using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public class IndexCompositePredicateFactory : IIndexCompositePredicateFactory
  {
    public async Task<Expression<Func<ResourceStore, bool>>> CompositeIndex(IPredicateFactory IPredicateFactory, SearchQueryComposite SearchQueryComposite)
    {
      var ResourceStorePredicate = LinqKit.PredicateBuilder.New<ResourceStore>(true);

      foreach (SearchQueryCompositeValue CompositeValue in SearchQueryComposite.ValueList)
      {
        if (!SearchQueryComposite.Modifier.HasValue)
        {
          if (CompositeValue.SearchQueryBaseList is null)
          {
            throw new ArgumentNullException(nameof(CompositeValue.SearchQueryBaseList));
          }
          ResourceStorePredicate = ResourceStorePredicate.Or(await IPredicateFactory.GetResourceStoreIndexPredicate(SearchQueryComposite.FhirVersionId, SearchQueryComposite.ResourceContext, CompositeValue.SearchQueryBaseList));
        }
        else
        {
          throw new ApplicationException($"Internal Server Error: The search query modifier: {SearchQueryComposite.Modifier.Value.GetCode()} is not supported for search parameter types of {SearchQueryComposite.SearchParamTypeId.GetCode()}.");
        }
      }
      return ResourceStorePredicate;
    }

  }
}
