using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.R4Fhir.Enums
{
  public class SummaryTypeMap : MapBase<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType>
  {
    private Dictionary<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType> _Map;
    protected override Dictionary<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType> Map { get { return _Map; } }
    public SummaryTypeMap()
    {
      _Map = new Dictionary<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType>();
      _Map.Add(SummaryType.Count, Hl7.Fhir.Rest.SummaryType.Count);
      _Map.Add(SummaryType.Data, Hl7.Fhir.Rest.SummaryType.Data);
      _Map.Add(SummaryType.False, Hl7.Fhir.Rest.SummaryType.False);
      _Map.Add(SummaryType.Text, Hl7.Fhir.Rest.SummaryType.Text);
      _Map.Add(SummaryType.True, Hl7.Fhir.Rest.SummaryType.True);
    }    
  }
}
