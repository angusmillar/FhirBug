using Bug.Common.Enums;
using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public async Task<ExpressionStarter<ResourceStore>> GetResourceStoreIndexPredicate(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, IList<ISearchQueryBase> SearchQueryList)
    {
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IEnumerable<ISearchQueryBase> NoChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is null);

      //Below must be true 'PredicateBuilder.New<ResourceStore>(true)' in order to get all resource when no query string
      ExpressionStarter<ResourceStore> Predicate = PredicateBuilder.New<ResourceStore>(true);
      foreach (var Search in NoChainedSearchQueryList)
      {
        switch (Search.SearchParamTypeId)
        {
          case Common.Enums.SearchParamType.Number:
            IResourceStorePredicateFactory.NumberIndex(Search).ForEach(x => Predicate = Predicate.Or(y => y.QuantityIndexList.Any(x.Compile())));
            break;
          case Common.Enums.SearchParamType.Date:
            IResourceStorePredicateFactory.DateTimeIndex(Search).ForEach(x => Predicate = Predicate.Or(y => y.DateTimeIndexList.Any(x.Compile())));
            break;
          case Common.Enums.SearchParamType.String:
            IResourceStorePredicateFactory.StringIndex(Search).ForEach(x => Predicate = Predicate.Or(y => y.StringIndexList.Any(x.Compile())));
            break;
          case Common.Enums.SearchParamType.Token:
            IResourceStorePredicateFactory.TokenIndex(Search).ForEach(x => Predicate = Predicate.Or(y => y.TokenIndexList.Any(x.Compile())));
            break;
          case Common.Enums.SearchParamType.Reference:
            (await IResourceStorePredicateFactory.ReferenceIndex(Search)).ForEach(x => Predicate = Predicate.Or(y => y.ReferenceIndexList.Any(x.Compile())));
            break;
          case Common.Enums.SearchParamType.Composite:
            Predicate = await IResourceStorePredicateFactory.CompositeIndex(this, Search);
            break;
          case Common.Enums.SearchParamType.Quantity:
            IResourceStorePredicateFactory.QuantityIndex(Search).ForEach(x => Predicate = Predicate.Or(y => y.QuantityIndexList.Any(x.Compile())));
            break;
          case Common.Enums.SearchParamType.Uri:
            IResourceStorePredicateFactory.UriIndex(Search).ForEach(x => Predicate = Predicate.Or(y => y.UriIndexList.Any(x.Compile())));
            break;
          case Common.Enums.SearchParamType.Special:
            throw new Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, new string[] { $"Attempt to search with a SearchParameter of type: {Common.Enums.SearchParamType.Special.GetCode()} which is not supported by this server." });
          default:
            break;
        }
        Predicate = Predicate.Extend(Predicate, PredicateOperator.And);
      }
      return Predicate;
    }

    //Remember that dbSet<ResourceStore>  is equal to IQueryable<ResourceStore>
    public async Task<IQueryable<ResourceStore>> ChainEntry1(AppDbContext AppDbContext, Common.Enums.ResourceType ParentResourceTypeContext, IList<ISearchQueryBase> SearchQueryList)
    {
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IQueryable<ResourceStore> ResourceStoreQuery = AppDbContext.Set<ResourceStore>();
      foreach (ISearchQueryBase ChainSearchQuery in ChainedSearchQueryList)
      {
        if (ChainSearchQuery.ChainedSearchParameter is null)
        {
          throw new NullReferenceException(nameof(ChainSearchQuery.ChainedSearchParameter));
        }

        ExpressionStarter<IndexReference> ExpressionStarterIndexReference = PredicateBuilder.New<IndexReference>(true);
        Expression<Func<IndexReference, bool>> Query = await ChainRecursion2(AppDbContext, ExpressionStarterIndexReference, ChainSearchQuery);

        //Below is not tested and probably fails!!!!
        ResourceStoreQuery = ResourceStoreQuery.Where(x => x.ReferenceIndexList.Any(Query.Compile()));        
      }
      return ResourceStoreQuery;
    }


    public async Task<List<ExpressionStarter<ResourceStore>>> ChainEntry(AppDbContext AppDbContext, Common.Enums.ResourceType ParentResourceTypeContext, IList<ISearchQueryBase> SearchQueryList)
    {
      var Result = new List<ExpressionStarter<ResourceStore>>();
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IQueryable<ResourceStore> ResourceStoreQuery = AppDbContext.Set<ResourceStore>();
      foreach (ISearchQueryBase ChainSearchQuery in ChainedSearchQueryList)
      {
        if (ChainSearchQuery.ChainedSearchParameter is null)
        {
          throw new NullReferenceException(nameof(ChainSearchQuery.ChainedSearchParameter));
        }

        ExpressionStarter<IndexReference> ExpressionStarterIndexReference = PredicateBuilder.New<IndexReference>(false);
        ExpressionStarterIndexReference = await ChainRecursion2(AppDbContext, ExpressionStarterIndexReference, ChainSearchQuery);
        ExpressionStarter<ResourceStore> ExpressionStarterResourceStore = PredicateBuilder.New<ResourceStore>(false);
        ExpressionStarterResourceStore = ExpressionStarterResourceStore.And(r => r.ReferenceIndexList.Any(ExpressionStarterIndexReference));
        Result.Add(ExpressionStarterResourceStore);        
      }
      return Result;
    }


    private async Task<Expression<Func<IndexReference, bool>>> ChainRecursion2(AppDbContext AppDbContext, Expression<Func<IndexReference, bool>> OtherIndexRefQuery, ISearchQueryBase ChainSearchQuery)
    {
      if (ChainSearchQuery.ChainedSearchParameter.ChainedSearchParameter is null)
      {
        switch (ChainSearchQuery.ChainedSearchParameter.SearchParamTypeId)
        {
          case Common.Enums.SearchParamType.String:
            List<Expression<Func<IndexString, bool>>> PredicateList = IResourceStorePredicateFactory.StringIndex(ChainSearchQuery.ChainedSearchParameter);
            IQueryable<IndexString> IndexString = AppDbContext.Set<IndexString>();
            ExpressionStarter<IndexString> PredicateGus = PredicateBuilder.New<IndexString>(true);
            foreach (var Pred in PredicateList)
            {
              Expression<Func<IndexString, bool>> Temp = Pred;
              PredicateGus = PredicateGus.Or(Temp);
            }

            OtherIndexRefQuery = OtherIndexRefQuery.And(x =>
              x.SearchParameterId == 1 &
              x.ResourceTypeId == ChainSearchQuery.TypeModifierResource!.Value &
              x.SearchParameterId == ChainSearchQuery.Id &
              x.VersionId == null &
              x.CanonicalVersionId == null &
              x.ResourceStore.ContainedId == null &
              IndexString.Where(PredicateGus).Select(ss => ss.ResourceStore.ResourceId).Contains(x.ResourceId));

            return OtherIndexRefQuery;
          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        ExpressionStarter<IndexReference> Resultx = await ChainRecursion2(AppDbContext, OtherIndexRefQuery, ChainSearchQuery.ChainedSearchParameter);

        IQueryable<IndexReference> IndexReference = AppDbContext.Set<IndexReference>();
        
        OtherIndexRefQuery = OtherIndexRefQuery.And(x =>
              x.SearchParameterId == 1 &
              x.ResourceTypeId == ChainSearchQuery.TypeModifierResource!.Value &
              x.SearchParameterId == ChainSearchQuery.Id &
              x.VersionId == null &
              x.CanonicalVersionId == null &
              x.ResourceStore.ContainedId == null &
              IndexReference.Where(Resultx).Select(ss => ss.ResourceStore.ResourceId).Contains(x.ResourceId));

        return OtherIndexRefQuery;
      }
    }



    private async Task<IQueryable<ResourceStore>> ChainRecursion(AppDbContext AppDbContext, IQueryable<ResourceStore> OtherResourceStoreQuery, ISearchQueryBase ChainSearchQuery)
    {
      if (ChainSearchQuery.ChainedSearchParameter is object)
      {
        //Recursively move through each child chain search query until at the end and child is null
        //The last one is the root search at the end of the chain, performer:Organization.name from the example.
        Bug.Common.Enums.ResourceType ChainedResourceTypeContext = ChainSearchQuery.ChainedSearchParameter.ResourceContext;
        IQueryable<ResourceStore> ResCurrentTypeContext = AppDbContext.Set<ResourceStore>();

        IQueryable<IndexReference> IndexReferenceContext = AppDbContext.Set<IndexReference>();
        IQueryable<IndexString> IndexStringContext = AppDbContext.Set<IndexString>();

        ResCurrentTypeContext = await ChainRecursion(AppDbContext, ResCurrentTypeContext, ChainSearchQuery.ChainedSearchParameter);

        Bug.Common.Enums.ResourceType ReferenceResourceType = ChainSearchQuery.TypeModifierResource.Value;
        int ReferenceSearchParameterId = ChainSearchQuery.ChainedSearchParameter.Id;
        //need to get the Key of the Primary Base URL somehow as hard coded here
        //int ReferencePrimaryServiceRootUrlId = IServiceBaseUrlCache.GetPrimaryAsync.Id;
        int ReferencePrimaryServiceRootUrlId = 1;


        OtherResourceStoreQuery = OtherResourceStoreQuery.Where(x =>
                                                                x.ReferenceIndexList.Any(y =>
                                                                                         y.ServiceBaseUrlId == 1 &
                                                                                         y.SearchParameterId == 1000 &
                                                                                         y.ResourceTypeId == Common.Enums.ResourceType.Observation &
                                                                                         y.CanonicalVersionId == null &
                                                                                         y.VersionId == null &
                                                                                         IndexReferenceContext.Where(g =>
                                                                                                                     g.ServiceBaseUrlId == 1 &
                                                                                                                     g.SearchParameterId == 2000 &
                                                                                                                     g.ResourceTypeId == Common.Enums.ResourceType.DiagnosticReport &
                                                                                                                     g.CanonicalVersionId == null &
                                                                                                                     g.VersionId == null &
                                                                                                                     IndexStringContext.Where(l =>
                                                                                                                                              l.SearchParameterId == 3000 &
                                                                                                                                              l.String == "Value").Select(s => s.ResourceStoreId).Contains(g.ResourceStoreId)).Select(q => q.ResourceStoreId).Contains(y.ResourceStoreId)));



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
                                                                                             f.ServiceBaseUrlId == ReferencePrimaryServiceRootUrlId)).Select(v => v.Id).Contains(c.Id)));
        return OtherResourceStoreQuery;
      }
      else
      {
        switch (ChainSearchQuery.SearchParamTypeId)
        {
          case Common.Enums.SearchParamType.Number:
            var Predicate2 = IResourceStorePredicateFactory.NumberIndex(ChainSearchQuery);
            IQueryable<IndexQuantity> IndexQuantity = AppDbContext.Set<IndexQuantity>();
            ExpressionStarter<IndexQuantity> PredicateGus = PredicateBuilder.New<IndexQuantity>(true);
            foreach (var Pred in Predicate2)
            {
              PredicateGus = PredicateGus.Or(Pred);
            }
            IndexQuantity = IndexQuantity.Where(PredicateGus);

            break;
          case Common.Enums.SearchParamType.Date:
            break;
          case Common.Enums.SearchParamType.String:
            break;
          case Common.Enums.SearchParamType.Token:
            break;
          case Common.Enums.SearchParamType.Reference:
            break;
          case Common.Enums.SearchParamType.Composite:
            break;
          case Common.Enums.SearchParamType.Quantity:
            break;
          case Common.Enums.SearchParamType.Uri:
            break;
          case Common.Enums.SearchParamType.Special:
            break;
          default:
            break;
        }


        var Predicate = await GetResourceStoreIndexPredicate(ChainSearchQuery.FhirVersionId, ChainSearchQuery.ResourceContext, new List<ISearchQueryBase>() { ChainSearchQuery });

        IQueryable<ResourceStore> ResCurrentTypeContext = AppDbContext.Set<ResourceStore>();
        ResCurrentTypeContext = ResCurrentTypeContext.AsExpandable().Where(Predicate);

        OtherResourceStoreQuery = OtherResourceStoreQuery
          .Where(x => x.ReferenceIndexList.Any(c => ResCurrentTypeContext.Select(v => v.Id).Contains(c.Id)));

        return OtherResourceStoreQuery;

      }
    }
  }
}
