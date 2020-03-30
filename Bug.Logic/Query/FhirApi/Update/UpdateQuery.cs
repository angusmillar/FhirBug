using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Bug.Logic.Query.FhirApi.Update
{  
  public class UpdateQuery : FhirApiResourceInstanceQuery
  {
    public UpdateQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId, FhirResource FhirResource)
      : base(HttpVerb, FhirVersion, RequestUri, HeaderDictionary, ResourceName, ResourceId)
    {
      this.FhirResource = FhirResource;      
    }

    public FhirResource FhirResource { get; set; }   
  }
}
