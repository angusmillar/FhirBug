using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.SearchQuery;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Logic.Service.Serach
{
  public class SearchService : ISearchService
  {
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly SearchServiceOutcome Outcome;
    public SearchService(IResourceStoreRepository IResourceStoreRepository)
    {
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.Outcome = new SearchServiceOutcome();
    }

    public SearchServiceOutcome Process(ISerachQueryServiceOutcome ISerachQueryServiceOutcome)
    {      
      IEnumerable<ISearchQueryBase> ChainedSearchQueryList = ISerachQueryServiceOutcome.SearchQueryList.Where(x => x.ChainedSearchParameter is object);
      IEnumerable<ISearchQueryBase> NoChainedSearchQueryList = ISerachQueryServiceOutcome.SearchQueryList.Where(x => x.ChainedSearchParameter is null);

     
      return Outcome;
    }
  }
}
