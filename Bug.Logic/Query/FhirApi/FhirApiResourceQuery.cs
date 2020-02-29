using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi
{
  public abstract class FhirApiResourceQuery : FhirApiQuery
  {
    public FhirApiResourceQuery(HttpVerb HttpVerb, FhirMajorVersion FhirMajorVersion, Uri RequestUri, FhirResource FhirResource, string ResourceName, Dictionary<string, StringValues> HeaderDictionary)
      : base(HttpVerb, FhirMajorVersion, RequestUri, HeaderDictionary)
    {
      this.FhirResource = FhirResource;
      this.ResourceName = ResourceName;
    }
    public FhirResource FhirResource { get; set; }
    public string ResourceName { get; set; }    
  }
}
