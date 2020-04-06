using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Bug.Logic.Query.FhirApi.Update
{  
  public class UpdateQuery : FhirApiResourceInstanceQuery
  {
    public UpdateQuery(HttpVerb HttpVerb, Common.Enums.FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> RequestQuery, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId, Common.FhirTools.FhirResource FhirResource)
      : base(HttpVerb, FhirVersion, RequestUri, RequestQuery, HeaderDictionary, ResourceName, ResourceId)
    {
      this.FhirResource = FhirResource;      
    }

    public Common.FhirTools.FhirResource FhirResource { get; set; }   
  }
}
