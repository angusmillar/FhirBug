using Bug.Common.Enums;
using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Data.Predicates
{
  public class PredicateFactory : IPredicateFactory
  {
    private readonly IResourceStorePredicateFactory IResourceStorePredicateFactory;
    private readonly IServiceBaseUrlCache IServiceBaseUrlCache;
    public PredicateFactory(IResourceStorePredicateFactory IResourceStorePredicateFactory,
      IServiceBaseUrlCache IServiceBaseUrlCache)
    {
      this.IResourceStorePredicateFactory = IResourceStorePredicateFactory;
      this.IServiceBaseUrlCache = IServiceBaseUrlCache;
    }

    public ExpressionStarter<ResourceStore> CurrentMainResource(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType)
    {
      return IResourceStorePredicateFactory.CurrentMainResource(fhirVersion, resourceType);      
    }

    public async Task<ExpressionStarter<ResourceStore>> GetIndexPredicate(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, IList<ISearchQueryBase> SearchQueryList)
    {
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IEnumerable<ISearchQueryBase> NoChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is null);

      //var CurrentMainResource = IResourceStorePredicateFactory.CurrentMainResource(fhirVersion, resourceType);
      //var Predicate = PredicateBuilder.New<ResourceStore>(CurrentMainResource);

      ExpressionStarter<ResourceStore> Predicate = PredicateBuilder.New<ResourceStore>();
      foreach (var Search in NoChainedSearchQueryList)
      {
        switch (Search.SearchParamTypeId)
        {
          case Common.Enums.SearchParamType.Number:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.NumberIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Date:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.DateTimeIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.String:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.StringIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Token:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.TokenIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Reference:
            Predicate = Predicate.Extend(await IResourceStorePredicateFactory.ReferenceIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Composite:
            Predicate = Predicate.Extend(await IResourceStorePredicateFactory.CompositeIndex(this, Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Quantity:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.QuantityIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Uri:
            Predicate = Predicate.Extend(IResourceStorePredicateFactory.UriIndex(Search), PredicateOperator.And);
            break;
          case Common.Enums.SearchParamType.Special:
            throw new Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, new string[] { $"Attempt to search with a SearchParameter of type: {Common.Enums.SearchParamType.Special.GetCode()} which is not supported by this server." });
          default:
            break;
        }
      }

      

      return Predicate;
    }

    //Remember that dbSet<ResourceStore>  is equal to IQueryable<ResourceStore>
    public async Task<IQueryable<ResourceStore>> ChainEntry(AppDbContext AppDbContext, Common.Enums.ResourceType ParentResourceTypeContext, IList<ISearchQueryBase> SearchQueryList)
    {
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IQueryable<ResourceStore> ResourceStoreQuery = AppDbContext.Set<ResourceStore>();
      foreach (ISearchQueryBase ChainSearchQuery in ChainedSearchQueryList)
      {
        if (ChainSearchQuery.ChainedSearchParameter is null)
        {
          throw new NullReferenceException(nameof(ChainSearchQuery.ChainedSearchParameter));
        }
                
        ResourceStoreQuery = await ChainRecursion(AppDbContext, ResourceStoreQuery, ChainSearchQuery);
      }
      return ResourceStoreQuery;
    }

    private async Task<IQueryable<ResourceStore>> ChainRecursion(AppDbContext AppDbContext, IQueryable<ResourceStore> OtherResourceStoreQuery, ISearchQueryBase ChainSearchQuery)
    {
      if (ChainSearchQuery.ChainedSearchParameter is object)
      {
        //Recursively move through each child chain search query until at the end and child is null
        //The last one is the root search at the end of the chain, performer:Organization.name from the example.
        Bug.Common.Enums.ResourceType ChainedResourceTypeContext = ChainSearchQuery.ChainedSearchParameter.ResourceContext;
        IQueryable<ResourceStore> ResCurrentTypeContext = AppDbContext.Set<ResourceStore>();
        ResCurrentTypeContext = await ChainRecursion(AppDbContext, ResCurrentTypeContext, ChainSearchQuery.ChainedSearchParameter);
        
        Bug.Common.Enums.ResourceType ReferenceResourceType = ChainSearchQuery.TypeModifierResource.Value;
        int ReferenceSearchParameterId = ChainSearchQuery.Id;
        //need to get the Key of the Primary Base URL somehow as hard coded here
        //int ReferencePrimaryServiceRootUrlId = IServiceBaseUrlCache.GetPrimaryAsync.Id;
        int ReferencePrimaryServiceRootUrlId = 1;

        OtherResourceStoreQuery = OtherResourceStoreQuery          
        .Where(x => 
               x.ReferenceIndexList.Any(c => 
                                        ResCurrentTypeContext.Where(a => 
                                                                    a.IsCurrent == true & 
                                                                    a.IsDeleted == false & 
                                                                    a.ContainedId == null &
                                                                    a.ReferenceIndexList.Any(f => 
                                                                                             f.ResourceTypeId == ReferenceResourceType & 
                                                                                             f.SearchParameterId == ReferenceSearchParameterId & 
                                                                                             f.VersionId == null & 
                                                                                             f.CanonicalVersionId == null &
                                                                                             f.ServiceBaseUrlId == ReferencePrimaryServiceRootUrlId)
                                                                    ).Select(v => v.Id).Contains(c.Id)));
        return OtherResourceStoreQuery;
      }
      else
      {

        var Predicate = await GetIndexPredicate(ChainSearchQuery.FhirVersionId, ChainSearchQuery.ResourceContext, new List<ISearchQueryBase>() { ChainSearchQuery });

        IQueryable<ResourceStore> ResCurrentTypeContext = AppDbContext.Set<ResourceStore>();
        ResCurrentTypeContext = ResCurrentTypeContext.AsExpandable().Where(Predicate);

        OtherResourceStoreQuery = OtherResourceStoreQuery          
          .Where(x => x.ReferenceIndexList
            .Any(c => ResCurrentTypeContext
              .Select(v => v.Id)
                .Contains(c.Id)));

        return ResCurrentTypeContext;

      }
    }
  }
}
