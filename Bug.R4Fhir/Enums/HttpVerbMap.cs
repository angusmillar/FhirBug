using Bug.Common.Enums;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Bug.R4Fhir.Enums
{
  public class HttpVerbMap : MapBase<HttpVerb, Hl7.Fhir.Model.Bundle.HTTPVerb>
  {
    private Dictionary<HttpVerb, Hl7.Fhir.Model.Bundle.HTTPVerb> _ForwardMap;
    private Dictionary<Bundle.HTTPVerb, HttpVerb> _ReverseMap;
    protected override Dictionary<HttpVerb, Bundle.HTTPVerb> ForwardMap { get { return _ForwardMap; } }
    protected override Dictionary<Bundle.HTTPVerb, HttpVerb> ReverseMap { get { return _ReverseMap; } }
    public HttpVerbMap()
    {
      _ForwardMap = new Dictionary<HttpVerb, Hl7.Fhir.Model.Bundle.HTTPVerb>();
      _ForwardMap.Add(HttpVerb.DELETE, Hl7.Fhir.Model.Bundle.HTTPVerb.DELETE);
      _ForwardMap.Add(HttpVerb.GET, Hl7.Fhir.Model.Bundle.HTTPVerb.GET);
      _ForwardMap.Add(HttpVerb.POST, Hl7.Fhir.Model.Bundle.HTTPVerb.POST);
      _ForwardMap.Add(HttpVerb.PUT, Hl7.Fhir.Model.Bundle.HTTPVerb.PUT);

      _ReverseMap = _ForwardMap.ToDictionary((i) => i.Value, (i) => i.Key);
    }
  }
}
