using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{
  public class IndexQuantity : IndexBase
  {
    public Bug.Common.Enums.QuantityComparator? Comparator { get; set; }
    public decimal? Quantity { get; set; }
    public string Code { get; set; }
    public string System { get; set; }
    public string Unit { get; set; }

    public Bug.Common.Enums.QuantityComparator? ComparatorHigh { get; set; }
    public decimal? QuantityHigh { get; set; }
    public string CodeHigh { get; set; }
    public string SystemHigh { get; set; }
    public string UnitHigh { get; set; }
  }
}
