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
    private readonly IIndexCompositePredicateFactory IIndexCompositePredicateFactory;



    public ResourceStorePredicateFactory(IIndexReferencePredicateFactory IIndexReferencePredicateFactory,
      IIndexStringPredicateFactory IIndexStringPredicateFactory,
      IIndexUriPredicateFactory IIndexUriPredicateFactory,
      IIndexQuantityPredicateFactory IIndexQuantityPredicateFactory,
      IIndexNumberPredicateFactory IIndexNumberPredicateFactory,
      IIndexTokenPredicateFactory IIndexTokenPredicateFactory,
      IIndexDateTimePredicateFactory IIndexDateTimePredicateFactory,
      IIndexCompositePredicateFactory IIndexCompositePredicateFactory)
    {
      this.IIndexStringPredicateFactory = IIndexStringPredicateFactory;
      this.IIndexReferencePredicateFactory = IIndexReferencePredicateFactory;
      this.IIndexUriPredicateFactory = IIndexUriPredicateFactory;
      this.IIndexQuantityPredicateFactory = IIndexQuantityPredicateFactory;
      this.IIndexNumberPredicateFactory = IIndexNumberPredicateFactory;
      this.IIndexTokenPredicateFactory = IIndexTokenPredicateFactory;
      this.IIndexDateTimePredicateFactory = IIndexDateTimePredicateFactory;
      this.IIndexCompositePredicateFactory = IIndexCompositePredicateFactory;
    }

    public Expression<Func<ResourceStore, bool>> CurrentMainResource(Bug.Common.Enums.FhirVersion fhirVersion, Bug.Common.Enums.ResourceType resourceType)
    {
      return x => x.FhirVersionId == fhirVersion && x.ResourceTypeId == resourceType && x.IsCurrent && !x.IsDeleted && x.ContainedId == null;
    }

    public List<Expression<Func<IndexString, bool>>> StringIndex(ISearchQueryBase SearchQueryBase)
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

    public List<Expression<Func<IndexUri, bool>>> UriIndex(ISearchQueryBase SearchQueryBase)
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

    public List<Expression<Func<IndexQuantity, bool>>> QuantityIndex(ISearchQueryBase SearchQueryBase)
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

    public List<Expression<Func<IndexQuantity, bool>>> NumberIndex(ISearchQueryBase SearchQueryBase)
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

    public List<Expression<Func<IndexToken, bool>>> TokenIndex(ISearchQueryBase SearchQueryBase)
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

    public async Task<List<Expression<Func<IndexReference, bool>>>> ReferenceIndex(ISearchQueryBase SearchQueryBase)
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

    public List<Expression<Func<IndexDateTime, bool>>> DateTimeIndex(ISearchQueryBase SearchQueryBase)
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

    public async Task<Expression<Func<ResourceStore, bool>>> CompositeIndex(IPredicateFactory IPredicateFactory, ISearchQueryBase SearchQueryBase)
    {
      if (SearchQueryBase is SearchQueryComposite SearchQueryComposite)
      {
        return await IIndexCompositePredicateFactory.CompositeIndex(IPredicateFactory, SearchQueryComposite);
      }
      else
      {
        throw new InvalidCastException($"Unable to cast a {nameof(ISearchQueryBase)} of type {SearchQueryBase.GetType().Name} to a {typeof(SearchQueryComposite).Name}");
      }
    }
    
  }
}
