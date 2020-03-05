using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Query.FhirApi.VRead
{
  
  public class VReadQuery : FhirApiResourceInstanceHistoryInstanceQuery
  {
    public VReadQuery(HttpVerb HttpVerb, FhirVersion FhirVersion, Uri RequestUri, Dictionary<string, StringValues> HeaderDictionary, string ResourceName, string ResourceId, int VersionId)
      : base(HttpVerb, FhirVersion, RequestUri, HeaderDictionary, ResourceName, ResourceId, VersionId) { }
  }
}
