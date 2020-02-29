using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.Update
{

  public class UpdateQuery : FhirApiResourceQuery
  {
    public UpdateQuery(HttpVerb HttpVerb, FhirMajorVersion FhirMajorVersion, Uri RequestUri, FhirResource FhirResource, string ResourceName, string ResourceId, Dictionary<string, StringValues> HeaderDictionary)
      : base(HttpVerb, FhirMajorVersion, RequestUri, FhirResource, ResourceName, HeaderDictionary)
    {
      this.ResourceId = ResourceId;
    }

    public string ResourceId { get; set; }
  }
}
