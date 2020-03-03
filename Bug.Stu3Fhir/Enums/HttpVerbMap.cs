using Bug.Common.Enums;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;


namespace Bug.Stu3Fhir.Enums
{
  public class HttpVerbMap : MapBase<HttpVerb, Hl7.Fhir.Model.Bundle.HTTPVerb>
  {
    private Dictionary<HttpVerb, Hl7.Fhir.Model.Bundle.HTTPVerb> _Map;
    protected override Dictionary<HttpVerb, Bundle.HTTPVerb> Map { get { return _Map; } }
    public HttpVerbMap()
    {
      _Map = new Dictionary<HttpVerb, Hl7.Fhir.Model.Bundle.HTTPVerb>();
      _Map.Add(HttpVerb.DELETE, Hl7.Fhir.Model.Bundle.HTTPVerb.DELETE);
      _Map.Add(HttpVerb.GET, Hl7.Fhir.Model.Bundle.HTTPVerb.GET);
      _Map.Add(HttpVerb.POST, Hl7.Fhir.Model.Bundle.HTTPVerb.POST);
      _Map.Add(HttpVerb.PUT, Hl7.Fhir.Model.Bundle.HTTPVerb.PUT);
    }

    
  }
}
