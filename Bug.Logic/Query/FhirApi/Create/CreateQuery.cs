using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.Create
{  
  public class CreateQuery : FhirApiResourceQuery
  {
    public CreateQuery(HttpVerb HttpVerb, Common.Enums.FhirVersion FhirMajorVersion, Uri RequestUri, Dictionary<string, StringValues> RequestQuery, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, Common.FhirTools.FhirResource FhirResource)
      : base(HttpVerb, FhirMajorVersion, RequestUri, RequestQuery, HeaderDictionary, ResourceName)
    {
      this.FhirResource = FhirResource;      
    }

    public Common.FhirTools.FhirResource FhirResource { get; set; }
    
  }
}
