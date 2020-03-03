using Bug.Common.Enums;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.Enums
{
  public class BundleTypeMap : MapBase<BundleType, Hl7.Fhir.Model.Bundle.BundleType>
  {
    private Dictionary<BundleType, Bundle.BundleType> _Map;
    protected override Dictionary<BundleType, Bundle.BundleType> Map { get { return _Map; } }
    public BundleTypeMap()      
    {
      _Map = new Dictionary<BundleType, Bundle.BundleType>();
      _Map.Add(BundleType.Batch, Bundle.BundleType.Batch);
      _Map.Add(BundleType.BatchResponse, Bundle.BundleType.BatchResponse);
      _Map.Add(BundleType.Collection, Bundle.BundleType.Collection);
      _Map.Add(BundleType.Document, Bundle.BundleType.Document);
      _Map.Add(BundleType.History, Bundle.BundleType.History);
      _Map.Add(BundleType.Message, Bundle.BundleType.Message);
      _Map.Add(BundleType.Searchset, Bundle.BundleType.Searchset);
      _Map.Add(BundleType.Transaction, Bundle.BundleType.Transaction);
      _Map.Add(BundleType.TransactionResponse, Bundle.BundleType.TransactionResponse);
    }
  }
}
