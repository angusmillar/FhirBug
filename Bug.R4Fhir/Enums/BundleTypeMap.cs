using Bug.Common.Enums;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace Bug.R4Fhir.Enums
{
  public class BundleTypeMap : MapBase<BundleType, Hl7.Fhir.Model.Bundle.BundleType>
  {
    private Dictionary<BundleType, Bundle.BundleType> _ForwardMap;
    private Dictionary<Bundle.BundleType, BundleType> _ReverseMap;
    protected override Dictionary<BundleType, Bundle.BundleType> ForwardMap { get { return _ForwardMap; } }
    protected override Dictionary<Bundle.BundleType, BundleType> ReverseMap { get { return _ReverseMap; } }
    public BundleTypeMap()
    {
      _ForwardMap = new Dictionary<BundleType, Bundle.BundleType>();
      _ForwardMap.Add(BundleType.Batch, Bundle.BundleType.Batch);
      _ForwardMap.Add(BundleType.BatchResponse, Bundle.BundleType.BatchResponse);
      _ForwardMap.Add(BundleType.Collection, Bundle.BundleType.Collection);
      _ForwardMap.Add(BundleType.Document, Bundle.BundleType.Document);
      _ForwardMap.Add(BundleType.History, Bundle.BundleType.History);
      _ForwardMap.Add(BundleType.Message, Bundle.BundleType.Message);
      _ForwardMap.Add(BundleType.Searchset, Bundle.BundleType.Searchset);
      _ForwardMap.Add(BundleType.Transaction, Bundle.BundleType.Transaction);
      _ForwardMap.Add(BundleType.TransactionResponse, Bundle.BundleType.TransactionResponse);

      //Auto Generate the reverse map
      _ReverseMap = _ForwardMap.ToDictionary((i) => i.Value, (i) => i.Key);

    }
  }
}
