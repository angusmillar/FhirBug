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
    public CreateQuery(HttpVerb HttpVerb, FhirMajorVersion FhirMajorVersion, Uri RequestUri, FhirResource FhirResource, string ResourceName, Dictionary<string, StringValues> HeaderDictionary)
      : base(HttpVerb, FhirMajorVersion, RequestUri, FhirResource, ResourceName, HeaderDictionary) { }
  }
}
