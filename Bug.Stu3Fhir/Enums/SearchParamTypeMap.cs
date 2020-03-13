using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Stu3Fhir.Enums
{
  public class SearchParamTypeMap : MapBase<Common.Enums.SearchParamType, Hl7.Fhir.Model.SearchParamType>
  {
    private Dictionary<Common.Enums.SearchParamType, Hl7.Fhir.Model.SearchParamType> _ForwardMap;
    private Dictionary<Hl7.Fhir.Model.SearchParamType, Common.Enums.SearchParamType> _ReverseMap;
    protected override Dictionary<Common.Enums.SearchParamType, Hl7.Fhir.Model.SearchParamType> ForwardMap { get { return _ForwardMap; } }
    protected override Dictionary<Hl7.Fhir.Model.SearchParamType, Common.Enums.SearchParamType> ReverseMap { get { return _ReverseMap; } }
    public SearchParamTypeMap()
    {
      _ForwardMap = new Dictionary<Common.Enums.SearchParamType, Hl7.Fhir.Model.SearchParamType>();
      _ForwardMap.Add(SearchParamType.Composite, Hl7.Fhir.Model.SearchParamType.Composite);
      _ForwardMap.Add(SearchParamType.Date, Hl7.Fhir.Model.SearchParamType.Date);
      _ForwardMap.Add(SearchParamType.Number, Hl7.Fhir.Model.SearchParamType.Number);
      _ForwardMap.Add(SearchParamType.Quantity, Hl7.Fhir.Model.SearchParamType.Quantity);
      _ForwardMap.Add(SearchParamType.Reference, Hl7.Fhir.Model.SearchParamType.Reference);      
      _ForwardMap.Add(SearchParamType.String, Hl7.Fhir.Model.SearchParamType.String);
      _ForwardMap.Add(SearchParamType.Token, Hl7.Fhir.Model.SearchParamType.Token);
      _ForwardMap.Add(SearchParamType.Uri, Hl7.Fhir.Model.SearchParamType.Uri);

      _ReverseMap = _ForwardMap.ToDictionary((i) => i.Value, (i) => i.Key);
    }    
  }
}
