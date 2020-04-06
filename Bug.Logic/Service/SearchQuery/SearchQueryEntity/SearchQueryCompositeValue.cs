using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryCompositeValue : SearchQueryValueBase
  {
    public List<ISearchQueryBase> SearchQueryBaseList { get; set; }

    public SearchQueryCompositeValue(bool IsMissing)
      :base(IsMissing)
    {      
      this.SearchQueryBaseList = new List<ISearchQueryBase>();
    }
  }
}
