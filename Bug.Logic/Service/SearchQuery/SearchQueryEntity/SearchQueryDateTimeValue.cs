using Bug.Common.Enums;
using System;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryDateTimeValue : SearchQueryValuePrefixBase
  {
    public SearchQueryDateTimeValue(bool IsMissing, SearchComparator? Prefix, DateTimePrecision? Precision, DateTime? Value)
      :base(IsMissing, Prefix)
    {      
      this.Precision = Precision;
      this.Value = Value;
    }

    public DateTimePrecision? Precision { get; set; }
    public DateTime? Value { get; set; }
  }
}
