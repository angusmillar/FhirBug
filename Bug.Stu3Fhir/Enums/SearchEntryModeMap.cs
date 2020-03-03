using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Stu3Fhir.Enums
{
  public class SearchEntryModeMap : MapBase<SearchEntryMode, Hl7.Fhir.Model.Bundle.SearchEntryMode>
  {
    private Dictionary<SearchEntryMode, Hl7.Fhir.Model.Bundle.SearchEntryMode> _Map;
    protected override Dictionary<SearchEntryMode, Hl7.Fhir.Model.Bundle.SearchEntryMode> Map { get { return _Map; } }
    public SearchEntryModeMap()
    {
      _Map = new Dictionary<SearchEntryMode, Hl7.Fhir.Model.Bundle.SearchEntryMode>();
      _Map.Add(SearchEntryMode.Include, Hl7.Fhir.Model.Bundle.SearchEntryMode.Include);
      _Map.Add(SearchEntryMode.Match, Hl7.Fhir.Model.Bundle.SearchEntryMode.Match);
      _Map.Add(SearchEntryMode.Outcome, Hl7.Fhir.Model.Bundle.SearchEntryMode.Outcome);
    }    
  }
}
