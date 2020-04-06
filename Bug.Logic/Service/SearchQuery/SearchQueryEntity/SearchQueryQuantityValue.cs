using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryQuantityValue : SearchQueryValuePrefixBase
  {
    public SearchQueryQuantityValue(bool IsMissing, SearchComparator? Prefix, string? system, string? code, int? precision, int? scale, decimal? value)
      :base(IsMissing, Prefix)
    {
      System = system;
      Code = code;
      Precision = precision;
      Scale = scale;
      Value = value;
    }

    public string? System { get; set; }
    public string? Code { get; set; }

    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public decimal? Value { get; set; }
  }
}
