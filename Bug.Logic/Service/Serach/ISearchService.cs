using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Serach
{
  public interface ISearchService
  {
    Task<IList<ResourceStore>> Process(ISerachQueryServiceOutcome ISerachQueryServiceOutcome);
  }
}