using Bug.Common.Enums;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Bug.Logic.Query.FhirApi.Delete
{  
  public class DeleteQuery : FhirApiResourceInstanceQuery
  {
    public DeleteQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId)
      : base(HttpVerb, FhirVersion, RequestUri, HeaderDictionary, ResourceName, ResourceId) { }
    
  }
}
