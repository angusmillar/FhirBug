using Bug.Common.Interfaces.CacheService;
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
  public class ResourceStorePredicateFactory : IResourceStorePredicateFactory
  {
    private readonly IIndexReferencePredicateFactory IIndexReferencePredicateFactory;
    private readonly IIndexStringPredicateFactory IIndexStringPredicateFactory;
    private readonly IIndexUriPredicateFactory IIndexUriPredicateFactory;
    private readonly IIndexQuantityPredicateFactory IIndexQuantityPredicateFactory;
    private readonly IIndexNumberPredicateFactory IIndexNumberPredicateFactory;
    private readonly IIndexTokenPredicateFactory IIndexTokenPredicateFactory;
    private readonly IIndexDateTimePredicateFactory IIndexDateTimePredicateFactory;



    public ResourceStorePredicateFactory(IIndexReferencePredicateFactory IIndexReferencePredicateFactory,
      IIndexStringPredicateFactory IIndexStringPredicateFactory,
      IIndexUriPredicateFactory IIndexUriPredicateFactory,
      IIndexQuantityPredicateFactory IIndexQuantityPredicateFactory,
      IIndexNumberPredicateFactory IIndexNumberPredicateFactory,
      IIndexTokenPredicateFactory IIndexTokenPredicateFactory,
      IIndexDateTimePredicateFactory IIndexDateTimePredicateFactory)
    {
      this.IIndexStringPredicateFactory = IIndexStringPredicateFactory;
      this.IIndexReferencePredicateFactory = IIndexReferencePredicateFactory;
      this.IIndexUriPredicateFactory = IIndexUriPredicateFactory;
      this.IIndexQuantityPredicateFactory = IIndexQuantityPredicateFactory;
      this.IIndexNumberPredicateFactory = IIndexNumberPredicateFactory;
      this.IIndexTokenPredicateFactory = IIndexTokenPredicateFactory;
      this.IIndexDateTimePredicateFactory = IIndexDateTimePredicateFactory;
    }

    public Expression<Func<ResourceStore, bool>> CurrentMainResource(Bug.Common.Enums.FhirVersion fhirVersion, Bug.Common.Enums.ResourceType resourceType)
    {
      return x => x.FhirVersionId == fhirVersion && x.ResourceTypeId == resourceType && x.IsCurrent && !x.IsDeleted && x.ContainedId == null;
    }

    public Expression<Func<ResourceStore, bool>> StringIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryString SearchQueryString)
      {
        return IIndexStringPredicateFactory.StringIndex(SearchQueryString);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryString).Name}");
      }
    }

    public Expression<Func<ResourceStore, bool>> UriIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryUri SearchQueryUri)
      {
        return IIndexUriPredicateFactory.UriIndex(SearchQueryUri);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryUri).Name}");
      }
    }

    public Expression<Func<ResourceStore, bool>> QuantityIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryQuantity SearchQueryQuantity)
      {
        return IIndexQuantityPredicateFactory.QuantityIndex(SearchQueryQuantity);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryQuantity).Name}");
      }
    }

    public Expression<Func<ResourceStore, bool>> NumberIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryNumber SearchQueryNumber)
      {
        return IIndexNumberPredicateFactory.NumberIndex(SearchQueryNumber);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryNumber).Name}");
      }
    }

    public Expression<Func<ResourceStore, bool>> TokenIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryToken SearchQueryToken)
      {
        return IIndexTokenPredicateFactory.TokenIndex(SearchQueryToken);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryToken).Name}");
      }
    }

    public async Task<Expression<Func<ResourceStore, bool>>> ReferenceIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryReference SearchQueryReference)
      {
        return await IIndexReferencePredicateFactory.ReferenceIndex(SearchQueryReference);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryReference).Name}");
      }
    }

    public Expression<Func<ResourceStore, bool>> DateTimeIndex(ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryDateTime SearchQueryDateTime)
      {
        return IIndexDateTimePredicateFactory.DateTimeIndex(SearchQueryDateTime);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryDateTime).Name}");
      }
    }

    
  }
}
