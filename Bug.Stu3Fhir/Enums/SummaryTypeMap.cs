using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Stu3Fhir.Enums
{
  public class SummaryTypeMap : MapBase<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType>
  {
    private Dictionary<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType> _ForwardMap;
    private Dictionary<Hl7.Fhir.Rest.SummaryType, Common.Enums.SummaryType> _ReverseMap;
    protected override Dictionary<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType> ForwardMap { get { return _ForwardMap; } }
    protected override Dictionary<Hl7.Fhir.Rest.SummaryType, Common.Enums.SummaryType> ReverseMap { get { return _ReverseMap; } }
    public SummaryTypeMap()
    {
      _ForwardMap = new Dictionary<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType>();
      _ForwardMap.Add(SummaryType.Count, Hl7.Fhir.Rest.SummaryType.Count);
      _ForwardMap.Add(SummaryType.Data, Hl7.Fhir.Rest.SummaryType.Data);
      _ForwardMap.Add(SummaryType.False, Hl7.Fhir.Rest.SummaryType.False);
      _ForwardMap.Add(SummaryType.Text, Hl7.Fhir.Rest.SummaryType.Text);
      _ForwardMap.Add(SummaryType.True, Hl7.Fhir.Rest.SummaryType.True);

      _ReverseMap = _ForwardMap.ToDictionary((i) => i.Value, (i) => i.Key);
    }    
  }
}
