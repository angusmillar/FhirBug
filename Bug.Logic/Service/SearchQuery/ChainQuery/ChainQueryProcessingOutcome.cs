using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.ChainQuery
{
  public class ChainQueryProcessingOutcome
  {
    public ChainQueryProcessingOutcome()
    {
      this.SearchQueryList = new List<ISearchQueryBase>();
      this.InvalidSearchQueryList = new List<Tools.InvalidSearchQueryParameter>();
    }

    public List<ISearchQueryBase> SearchQueryList { get; set; }
    public List<Tools.InvalidSearchQueryParameter> InvalidSearchQueryList { get; set; }
  }
}
