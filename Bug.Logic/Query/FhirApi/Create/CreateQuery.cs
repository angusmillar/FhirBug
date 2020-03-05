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
    public CreateQuery(HttpVerb HttpVerb, FhirVersion FhirMajorVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, FhirResource FhirResource)
      : base(HttpVerb, FhirMajorVersion, RequestUri, HeaderDictionary, ResourceName)
    {
      this.FhirResource = FhirResource;
    }

    public FhirResource FhirResource { get; set; }
  }
}
