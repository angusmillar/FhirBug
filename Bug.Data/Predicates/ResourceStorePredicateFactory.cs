using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Bug.Data.Predicates
{
  public static class ResourceStorePredicateFactory
  {
    public static Expression<Func<ResourceStore, bool>> CurrentMainResource(Bug.Common.Enums.FhirVersion fhirVersion, Bug.Common.Enums.ResourceType resourceType)
    {
      return x => x.FhirVersionId == fhirVersion && x.ResourceTypeId == resourceType && x.IsCurrent && !x.IsDeleted && x.ContainedId == null;
    }

    public static Expression<Func<ResourceStore, bool>> StringIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryString SearchQueryString)
      {
        return IndexStringPredicateFactory.StringIndex(SearchQueryString);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryString).Name}");
      }          
    }

    public static Expression<Func<ResourceStore, bool>> UriIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryUri SearchQueryUri)
      {
        return IndexUriPredicateFactory.UriIndex(SearchQueryUri);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryUri).Name}");
      }      
    }

    public static Expression<Func<ResourceStore, bool>> QuantityIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryQuantity SearchQueryQuantity)
      {
        return IndexQuantityPredicateFactory.QuantityIndex(SearchQueryQuantity);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryQuantity).Name}");
      }
    }

    public static Expression<Func<ResourceStore, bool>> NumberIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryNumber SearchQueryNumber)
      {
        return IndexNumberPredicateFactory.NumberIndex(SearchQueryNumber);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryNumber).Name}");
      }
    }

    public static Expression<Func<ResourceStore, bool>> TokenIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryToken SearchQueryToken)
      {
        return IndexTokenPredicateFactory.TokenIndex(SearchQueryToken);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryToken).Name}");
      }
    }
  }
}
