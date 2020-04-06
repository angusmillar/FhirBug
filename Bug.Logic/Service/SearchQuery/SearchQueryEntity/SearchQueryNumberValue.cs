using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryNumberValue : SearchQueryValuePrefixBase
  {
    public SearchQueryNumberValue(bool IsMissing, SearchComparator? Prefix, int? Precision, int? Scale, decimal? Value)
      :base(IsMissing, Prefix)
    {      
      this.Precision = Precision;
      this.Scale = Scale;
      this.Value = Value;
    }

    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public decimal? Value { get; set; }
  }
}
