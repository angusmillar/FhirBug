using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Common.Dto.Indexing
{
  public class IndexQuantity : IndexBase
  {
    public IndexQuantity(int fkSearchParameterId) 
      : base(fkSearchParameterId)
    {
    }

    public QuantityComparator? Comparator { get; set; }
    public decimal? Quantity { get; set; }
    public string? Code { get; set; }
    public string? System { get; set; }
    public string? Unit { get; set; }

    public QuantityComparator? ComparatorHigh { get; set; }
    public decimal? QuantityHigh { get; set; }
    public string? CodeHigh { get; set; }
    public string? SystemHigh { get; set; }
    public string? UnitHigh { get; set; }
  }
}
