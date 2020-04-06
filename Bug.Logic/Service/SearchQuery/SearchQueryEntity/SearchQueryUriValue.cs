using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryUriValue : SearchQueryValueBase
  {
    public SearchQueryUriValue(bool IsMissing, Uri? Value)
    :base(IsMissing)
    {      
      this.Value = Value;
    }

    public Uri? Value { get; set; }
  }
}
