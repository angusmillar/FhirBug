using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryStringValue : SearchQueryValueBase
  {
    public SearchQueryStringValue(bool IsMissing, string? Value)
      :base(IsMissing)
    {      
      this.Value = Value;
    }

    public string? Value { get; set; }
  }
}
