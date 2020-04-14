using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.SearchQuery;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task<IList<ResourceStore>> Process(ISerachQueryServiceOutcome ISerachQueryServiceOutcome)
    {

      return await IResourceStoreRepository.GetSearch(ISerachQueryServiceOutcome.FhirVersion, ISerachQueryServiceOutcome.ResourceContext, ISerachQueryServiceOutcome.SearchQueryList);
      
    }
  }
}
