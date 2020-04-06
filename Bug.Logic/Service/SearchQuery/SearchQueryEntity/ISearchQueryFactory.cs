using Bug.Logic.DomainModel;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public interface ISearchQueryFactory
  {
    Task<IList<ISearchQueryBase>> Create(Common.Enums.ResourceType ResourceContext, SearchParameter searchParameter, KeyValuePair<string, StringValues> Parameter, bool IsChainedReferance = false);
  }
}