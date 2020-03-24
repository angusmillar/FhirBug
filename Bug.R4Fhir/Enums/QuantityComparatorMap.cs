using Bug.Common.Enums;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace Bug.R4Fhir.Enums
{
  public class QuantityComparatorMap : MapBase<QuantityComparator, Quantity.QuantityComparator>
  {
    private Dictionary<QuantityComparator, Quantity.QuantityComparator> _ForwardMap;
    private Dictionary<Quantity.QuantityComparator, QuantityComparator> _ReverseMap;
    protected override Dictionary<QuantityComparator, Quantity.QuantityComparator> ForwardMap { get { return _ForwardMap; } }
    protected override Dictionary<Quantity.QuantityComparator, QuantityComparator> ReverseMap { get { return _ReverseMap; } }
    public QuantityComparatorMap()
    {
      _ForwardMap = new Dictionary<QuantityComparator, Quantity.QuantityComparator > ();
      _ForwardMap.Add(QuantityComparator.GreaterOrEqual, Quantity.QuantityComparator.GreaterOrEqual);
      _ForwardMap.Add(QuantityComparator.GreaterThan, Quantity.QuantityComparator.GreaterThan);
      _ForwardMap.Add(QuantityComparator.LessOrEqual, Quantity.QuantityComparator.LessOrEqual);
      _ForwardMap.Add(QuantityComparator.LessThan, Quantity.QuantityComparator.LessThan);

      //Auto Generate the reverse map
      _ReverseMap = _ForwardMap.ToDictionary((i) => i.Value, (i) => i.Key);

    }
  }
}
